using System;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore.Playground
{
    public class PlaygroundOptions
    {
        private PathString _path = "/ui/playground";
        private PathString _queryPath = "/";
        private PathString _subscriptionPath = "/ws";
        
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
            }
        }

        public PathString QueryPath
        {
            get => _queryPath;
            set
            {
                if (!value.HasValue)
                {
                    throw new ArgumentException(
                        "The query-path cannot be empty.");
                }

                _queryPath = value;
                _subscriptionPath = value + "/ws";
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
    }
}
