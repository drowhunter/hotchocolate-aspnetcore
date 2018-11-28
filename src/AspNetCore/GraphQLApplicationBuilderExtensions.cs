using System;
using System.Threading.Tasks;
using HotChocolate.AspNetCore;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace HotChocolate
{
    public static class GraphQLApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGraphQL(
            this IApplicationBuilder applicationBuilder)
        {
            return UseGraphQL(applicationBuilder,
                new GraphQLMiddlewareOptions());
        }

        public static IApplicationBuilder UseGraphQL(
            this IApplicationBuilder applicationBuilder,
            PathString path)
        {
            var options = new GraphQLMiddlewareOptions
            {
                Path = path.HasValue ? path : new PathString("/")
            };

            return UseGraphQL(applicationBuilder, options);
        }

        public static IApplicationBuilder UseGraphQL(
            this IApplicationBuilder applicationBuilder,
            GraphQLMiddlewareOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return applicationBuilder
                .UseMiddleware<PostQueryMiddleware>(options)
                .UseMiddleware<GetQueryMiddleware>(options)
                .UseMiddleware<SubscriptionMiddleware>(options)
                .UseMiddleware<SchemaMiddleware>(options);
        }

        public static IApplicationBuilder UseGraphQL(
            this IApplicationBuilder applicationBuilder,
            ISchema schema,
            GraphQLMiddlewareOptions options)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var executer = new QueryExecuter(schema);

            return applicationBuilder
                .UseMiddleware<PostQueryMiddleware>(executer, options)
                .UseMiddleware<GetQueryMiddleware>(executer, options)
                .UseMiddleware<SubscriptionMiddleware>(executer, options)
                .UseMiddleware<SchemaMiddleware>(executer, options);
        }
    }
}
