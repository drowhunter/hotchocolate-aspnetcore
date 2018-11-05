using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authorization
{
    public class AuthorizeDirectiveType
        : DirectiveType<AuthorizeDirective>
    {
        protected override void Configure(
            IDirectiveTypeDescriptor<AuthorizeDirective> descriptor)
        {
            descriptor.Name("authorize");

            descriptor.Location(DirectiveLocation.Object)
                .Location(DirectiveLocation.FieldDefinition);

            descriptor.Middleware(
                next => context => AuthorizeAsync(context, next));
        }

        private static async Task AuthorizeAsync(
            IDirectiveContext context,
            DirectiveDelegate next)
        {
            var authorizeService = context.Service<IAuthorizationService>();
            var httpContext = context.Service<HttpContext>();
            var directive = context.Directive.ToObject<AuthorizeDirective>();

            bool allowed = IsInRoles(httpContext.User, directive.Roles);

            if (allowed && !string.IsNullOrEmpty(directive.Policy))
            {
                allowed = await authorizeService
                    .AuthorizeAsync(httpContext.User, directive.Policy);
            }

            if (allowed)
            {
                await next(context);
            }
            else
            {
                context.Result = QueryError.CreateFieldError(
                    "The current user is not authorized to " +
                    "access this resource.",
                    context.Path,
                    context.FieldSelection);
            }
        }

        private static bool IsInRoles(
            IPrincipal principal,
            IReadOnlyCollection<string> roles)
        {
            if (roles != null)
            {
                foreach (string role in roles)
                {
                    if (!principal.IsInRole(role))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
