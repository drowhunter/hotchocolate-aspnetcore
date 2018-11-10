using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.Execution;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore
{
    public class SubscriptionMiddleware
    {
        private readonly RequestDelegate _next;

        public SubscriptionMiddleware(
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
            if (context.WebSockets.IsWebSocketRequest
                && context.IsValidPath(Options.SubscriptionPath))
            {
                var session = await WebSocketSession
                    .TryCreateAsync(context, Executer)
                    .ConfigureAwait(false); ;

                if (session != null)
                {
                    await session.StartAsync(context.RequestAborted)
                        .ConfigureAwait(false);
                }
            }
            else
            {
                await _next(context).ConfigureAwait(false);
            }
        }
    }
}
