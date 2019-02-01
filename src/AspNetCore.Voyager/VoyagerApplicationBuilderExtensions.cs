using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;

namespace HotChocolate.AspNetCore.Voyager
{
    public static class VoyagerApplicationBuilderExtensions
    {
        private const string ResourcesNamespace =
            "HotChocolate.AspNetCore.Voyager.Resources";

        /// <summary>
        /// Add GraphQL Voyager UI to address '/Voyager'
        /// </summary>
        /// <param name="applicationBuilder"></param>
        public static void UseVoyager(
            this IApplicationBuilder applicationBuilder)
        {
            UseVoyager(applicationBuilder, new VoyagerOptions());
        }

        /// <summary>
        /// Add GraphQL Voyager UI to address '/Voyager'
        /// relative to GraphQL endpoint
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="queryPath"></param>
        public static void UseVoyager(
           this IApplicationBuilder applicationBuilder,
           PathString queryPath)
        {
            UseVoyager(applicationBuilder, new VoyagerOptions
            {
                QueryPath = queryPath,
                Path = queryPath + "/Voyager"
            });
        }

        /// <summary>
        /// Add GraphQL Voyager UI to specified address
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="queryPath"></param>
        /// <param name="uiPath"></param>
        public static void UseVoyager(
            this IApplicationBuilder applicationBuilder,
            PathString queryPath,
            PathString uiPath)
        {
            UseVoyager(applicationBuilder, new VoyagerOptions
            {
                QueryPath = queryPath,
                Path = uiPath
            });
        }

        /// <summary>
        /// Add GraphQL Voyager UI to specified address
        /// </summary>
        /// <param name="applicationBuilder"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void UseVoyager(
            this IApplicationBuilder applicationBuilder,
            VoyagerOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            applicationBuilder.UseVoyagerSettingsMiddleware(options);
            applicationBuilder.UseVoyagerFileServer(options.Path);
        }

        private static void UseVoyagerSettingsMiddleware(
           this IApplicationBuilder applicationBuilder,
           VoyagerOptions options)
        {
            applicationBuilder.Map(
                options.Path.Add("/settings.js"),
                app => app.UseMiddleware<SettingsMiddleware>(options));
        }

        private static void UseVoyagerFileServer(
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
            Type type = typeof(VoyagerApplicationBuilderExtensions);

            return new EmbeddedFileProvider(
                type.Assembly,
                ResourcesNamespace);
        }
    }
}
