using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore
{
    public delegate Task OnCreateRequestAsync(
        HttpContext context,
        Execution.QueryRequest request,
        IDictionary<string, object> properties,
        CancellationToken cancellationToken);

    public delegate Task<ConnectionStatus> OnConnectWebSocketAsync(
        HttpContext context,
        IDictionary<string, object> properties,
        CancellationToken cancellationToken);

    public class GraphQLMiddlewareOptions
    {
        private PathString _path = "/";
        private PathString _subscriptionPath;

        public PathString Path
        {
            get => _path;
            set
            {
                if (!value.HasValue)
                {
                    throw new ArgumentException(
                        "The path cannot be empty.");
                }

                _path = value;
                SubscriptionPath = value + "/ws";
            }
        }

        public PathString SubscriptionPath
        {
            get => _subscriptionPath;
            set
            {
                if (!value.HasValue)
                {
                    throw new ArgumentException(
                        "The subscription-path cannot be empty.");
                }

                _subscriptionPath = value;
            }
        }

        public OnConnectWebSocketAsync OnConnectWebSocket { get; set; }

        public OnCreateRequestAsync OnCreateRequest { get; set; }
    }
}
