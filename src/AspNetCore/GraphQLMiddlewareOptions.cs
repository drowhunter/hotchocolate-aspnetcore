using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore
{
    public delegate Task OnCreateRequestAsync(
        HttpContext context,
        Execution.QueryRequest request,
        IDictionary<string, object> properties);

    public delegate Task OnConnectWebSocketAsync(
        IDictionary<string, object> properties);

    public class GraphQLMiddlewareOptions
    {
        private PathString _path = "/";
        private PathString _subscriptionPath;

        public int QueryCacheSize { get; set; } = 100;

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

        public OnConnectWebSocketAsync OnConnect { get; set; }

        public OnCreateRequestAsync OnCreateRequest { get; set; }
    }
}
