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
        private const string _postMethod = "POST";

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
                context.Request.Method, _postMethod,
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
                    .DeserializeVariables(request.Variables),
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
                string json = await reader.ReadToEndAsync()
                    .ConfigureAwait(false);

                return JsonConvert.DeserializeObject<QueryRequestDto>(json);
            }
        }
    }
}
