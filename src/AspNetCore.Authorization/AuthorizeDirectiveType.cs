using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using HotChocolate.Execution;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace HotChocolate.AspNetCore
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
            var principal = context.CustomProperty<ClaimsPrincipal>(
                nameof(ClaimsPrincipal));
            var directive = context.Directive.ToObject<AuthorizeDirective>();

            bool allowed = IsInRoles(principal, directive.Roles);

            if (allowed && !string.IsNullOrEmpty(directive.Policy))
            {
                AuthorizationResult result = await authorizeService
                    .AuthorizeAsync(principal, directive.Policy);
                allowed = result.Succeeded;
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
