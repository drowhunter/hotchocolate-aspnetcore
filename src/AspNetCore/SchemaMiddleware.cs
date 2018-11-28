using System;
using System.IO;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Language;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore
{
    internal sealed class SchemaMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly QueryExecuter _queryExecuter;
        private readonly string _path;

        public SchemaMiddleware(
            RequestDelegate next,
            QueryExecuter queryExecuter,
            GraphQLMiddlewareOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _next = next
                ?? throw new ArgumentNullException(nameof(next));
            _queryExecuter = queryExecuter
                ?? throw new ArgumentNullException(nameof(queryExecuter));
            _path = options.Path.Add("/schema");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Method.EqualsOrdinal(HttpMethods.Get)
                && context.IsValidPath(_path))
            {
                context.Response.ContentType = "application/graphql";
                context.Response.Headers.Add(
                    "Content-Disposition",
                    "attachment; filename=\"schema.graphql\"");

                using (var streamWriter = new StreamWriter(
                    context.Response.Body))
                {
                    SchemaSerializer.Serialize(
                        _queryExecuter.Schema,
                        streamWriter);

                    await streamWriter.FlushAsync();
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
