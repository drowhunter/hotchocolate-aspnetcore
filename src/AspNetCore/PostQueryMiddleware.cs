using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ExecQueryRequest = HotChocolate.Execution.QueryRequest;

namespace HotChocolate.AspNetCore
{
    public class PostQueryMiddleware
        : QueryMiddlewareBase
    {
        public PostQueryMiddleware(
            RequestDelegate next,
            QueryExecuter queryExecuter,
            GraphQLMiddlewareOptions options)
            : base(next, queryExecuter, options)
        {
        }

        protected override bool CanHandleRequest(HttpContext context)
        {
            return string.Equals(
                context.Request.Method,
                HttpMethods.Post,
                StringComparison.Ordinal);
        }

        protected override async Task<ExecQueryRequest> CreateQueryRequest(
            HttpContext context)
        {
            QueryRequestDto request = await ReadRequestAsync(context)
                .ConfigureAwait(false); ;

            return new ExecQueryRequest(
                request.Query, request.OperationName)
            {
                VariableValues = QueryMiddlewareUtilities
                    .ToDictionary(request.Variables),
                Services = QueryMiddlewareUtilities
                    .CreateRequestServices(context)
            };
        }

        private static async Task<QueryRequestDto> ReadRequestAsync(
            HttpContext context)
        {
            using (StreamReader reader = new StreamReader(
                context.Request.Body, Encoding.UTF8))
            {
                string content = await reader.ReadToEndAsync()
                    .ConfigureAwait(false);

                switch (context.Request.ContentType.Split(';')[0])
                {
                    case ContentType.Json:
                        return JsonConvert
                            .DeserializeObject<QueryRequestDto>(content);

                    case ContentType.GraphQL:
                        return new QueryRequestDto { Query = content };

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
