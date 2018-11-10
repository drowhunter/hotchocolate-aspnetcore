using System;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore
{
    internal static class HttpContextExtensions
    {
        public static bool IsValidPath(this HttpContext context, PathString path)
        {
            return context.Request.Path.StartsWithSegments(
                path, out _, out _);
        }
    }
}
