using HotChocolate.AspNetCore;

namespace HotChocolate
{
    public static class SchemaConfigurationExtensions
    {
        public static void RegisterAuthorizeDirectiveType(
            this ISchemaConfiguration configuration)
        {
            configuration.RegisterDirective<AuthorizeDirectiveType>();
        }
    }
}
