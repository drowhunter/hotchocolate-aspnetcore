using System;
using System.Reflection;
using HotChocolate.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace HotChocolate
{
    public static class GraphiQLApplicationBuilderExtensions
    {
        private const string _resourcesNamespace =
            "HotChocolate.AspNetCore.Resources";

        public static void UseGraphiQL(
            this IApplicationBuilder applicationBuilder)
        {
            UseGraphiQL(applicationBuilder, new GraphiQLOptions());
        }

        public static void UseGraphiQL(
           this IApplicationBuilder applicationBuilder,
           PathString queryPath)
        {
            UseGraphiQL(applicationBuilder, new GraphiQLOptions
            {
                QueryPath = queryPath,
                Path = queryPath + "/ui"
            });
        }

         public static void UseGraphiQL(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath,
            PathString uiPath)
        {
            UseGraphiQL(applicationBuilder, new GraphiQLOptions
            {
                QueryPath = queryPath,
                Path = uiPath
            });
        }

        public static void UseGraphiQL(
            this IApplicationBuilder applicationBuilder,
            GraphiQLOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            applicationBuilder.UseGraphiQLSettingsMiddleware(options);
            applicationBuilder.UseGraphiQLFileServer(options.Path);
        }

        private static void UseGraphiQLSettingsMiddleware(
           this IApplicationBuilder applicationBuilder,
           GraphiQLOptions options)
        {
            applicationBuilder.Map(options.Path.Add("/settings.js"),
                app => app.UseMiddleware<SettingsMiddleware>(options));
        }

        private static void UseGraphiQLFileServer(
            this IApplicationBuilder applicationBuilder,
            string path)
        {
            var fileServerOptions = new FileServerOptions
            {
                RequestPath = path,
                FileProvider = CreateFileProvider(),
                EnableDefaultFiles = true,
                StaticFileOptions =
                {
                    ContentTypeProvider = new FileExtensionContentTypeProvider()
                }
            };

            applicationBuilder.UseFileServer(fileServerOptions);
        }

        private static IFileProvider CreateFileProvider()
        {
            Type type = typeof(GraphiQLApplicationBuilderExtensions);

            return new EmbeddedFileProvider(
                type.Assembly,
                _resourcesNamespace);
        }
    }
}
