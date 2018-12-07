using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace HotChocolate.AspNetCore.Playground
{
    public static class PlaygroundApplicationBuilderExtensions
    {
        private const string ResourcesNamespace =
            "HotChocolate.AspNetCore.Playground.Resources";

        /// <summary>
        /// Add GraphQL Playground UI to address '/playground'
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void UsePlayground(
            this IApplicationBuilder applicationBuilder)
        {
            UsePlayground(applicationBuilder, new PlaygroundOptions());
        }

        /// <summary>
        /// Add GraphQL Playground UI to address '/playground'
        /// relative to GraphQL endpoint
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="queryPath"></param>
        public static void UsePlayground(
           this IApplicationBuilder applicationBuilder,
           PathString queryPath)
        {
            UsePlayground(applicationBuilder, new PlaygroundOptions
            {
                QueryPath = queryPath,
                Path = queryPath + "/playground"
            });
        }

        /// <summary>
        /// Add GraphQL Playground UI to specified address
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="queryPath"></param>
        /// <param name="uiPath"></param>
        public static void UsePlayground(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath,
            PathString uiPath)
        {
            UsePlayground(applicationBuilder, new PlaygroundOptions
            {
                QueryPath = queryPath,
                Path = uiPath
            });
        }

        /// <summary>
        /// Add GraphQL Playground UI to specified address
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void UsePlayground(
            this IApplicationBuilder applicationBuilder,
            PlaygroundOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            applicationBuilder.UsePlaygroundSettingsMiddleware(options);
            applicationBuilder.UsePlaygroundFileServer(options.Path);
        }

        private static void UsePlaygroundSettingsMiddleware(
           this IApplicationBuilder applicationBuilder,
           PlaygroundOptions options)
        {
            applicationBuilder.Map(
                options.Path.Add("/settings.js"),
                app => app.UseMiddleware<SettingsMiddleware>(options));
        }

        private static void UsePlaygroundFileServer(
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
            Type type = typeof(PlaygroundApplicationBuilderExtensions);

            return new EmbeddedFileProvider(
                type.Assembly,
                ResourcesNamespace);
        }
    }
}
