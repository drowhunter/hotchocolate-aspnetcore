using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HotChocolate.AspNetCore.Playground
{
    internal sealed class SettingsMiddleware
    {
        private readonly string _queryPath;
        private readonly string _subscriptionPath;

        public SettingsMiddleware(
            RequestDelegate next,
            PlaygroundOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _queryPath = options.QueryPath;
            _subscriptionPath = options.SubscriptionPath;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string queryUrl = BuildUrl(context.Request, false, _queryPath);
            string subscriptionUrl =
                BuildUrl(context.Request, true, _subscriptionPath);

            context.Response.ContentType = "application/javascript";

            await context.Response.WriteAsync($@"
                window.Settings = {{
                    url: ""{queryUrl}"",
                    subscriptionUrl: ""{subscriptionUrl}"",
                }}
            ", context.RequestAborted);
        }

        private static string BuildUrl(
            HttpRequest request,
            bool websocket,
            string path)
        {
            string scheme = request.Scheme;

            if (websocket)
            {
                scheme = request.IsHttps ? "wss" : "ws";
            }

            return UriHelper
                .BuildAbsolute(scheme, request.Host, path)
                .TrimEnd('/');
        }
    }
}
