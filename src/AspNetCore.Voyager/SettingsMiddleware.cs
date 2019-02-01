using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace HotChocolate.AspNetCore.Voyager
{
    internal sealed class SettingsMiddleware
    {
        private readonly string _queryPath;
        
        public SettingsMiddleware(
            RequestDelegate next,
            VoyagerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _queryPath = options.QueryPath;
            
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string queryUrl = BuildUrl(context.Request,  _queryPath);
           
            context.Response.ContentType = "application/javascript";

            await context.Response.WriteAsync($@"
                window.Settings = {{
                    url: ""{queryUrl}"",
                }}
            ", context.RequestAborted);
        }

        private static string BuildUrl(
            HttpRequest request,
            
            string path)
        {
            string scheme = request.Scheme;

           

            return UriHelper
                .BuildAbsolute(scheme, request.Host, path)
                .TrimEnd('/');
        }
    }
}
