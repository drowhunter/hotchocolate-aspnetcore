using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore
{
    public abstract class QueryMiddlewareBase
    {
        private readonly RequestDelegate _next;

        public QueryMiddlewareBase(
            RequestDelegate next,
            QueryExecuter queryExecuter,
            GraphQLMiddlewareOptions options)
        {
            _next = next
                ?? throw new ArgumentNullException(nameof(next));
            Executer = queryExecuter
                ?? throw new ArgumentNullException(nameof(queryExecuter));
            Options = options
                ?? throw new ArgumentNullException(nameof(options));
        }

        protected QueryExecuter Executer { get; }

        protected GraphQLMiddlewareOptions Options { get; }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.IsValidPath(Options.Path) && CanHandleRequest(context))
            {
                await HandleRequestAsync(context, Executer)
                    .ConfigureAwait(false);
            }
            else
            {
                await _next(context).ConfigureAwait(false);
            }
        }

        protected abstract bool CanHandleRequest(HttpContext context);

        protected T GetService<T>(HttpContext context) =>
            (T)context.RequestServices.GetService(typeof(T));

        private async Task<QueryRequest> CreateQueryRequestInternal(
            HttpContext context)
        {
            QueryRequest request = await CreateQueryRequest(context)
                .ConfigureAwait(false); ;

            var requestProperties = new Dictionary<string, object>();
            requestProperties[nameof(ClaimsPrincipal)] = context.User;
            request.Properties = requestProperties;

            var onCreateRequest = Options.OnCreateRequest
                ?? GetService<OnCreateRequestAsync>(context);

            if (onCreateRequest != null)
            {
                await onCreateRequest(context, request, requestProperties)
                    .ConfigureAwait(false);
            }

            return request;
        }

        protected abstract Task<QueryRequest> CreateQueryRequest(
            HttpContext context);

        private async Task HandleRequestAsync(
            HttpContext context,
            QueryExecuter queryExecuter)
        {
            QueryRequest request =
                await CreateQueryRequestInternal(context)
                    .ConfigureAwait(false);

            IExecutionResult result = await queryExecuter
                .ExecuteAsync(request, context.RequestAborted)
                .ConfigureAwait(false);

            await WriteResponseAsync(context.Response, result)
                .ConfigureAwait(false);
        }

        private async Task WriteResponseAsync(
            HttpResponse response,
            IExecutionResult executionResult)
        {
            if (executionResult is IQueryExecutionResult queryResult)
            {
                string json = queryResult.ToJson();
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                await response.Body.WriteAsync(buffer, 0, buffer.Length)
                    .ConfigureAwait(false); ;
            }
        }
    }
}
