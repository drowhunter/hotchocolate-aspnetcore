using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HotChocolate.AspNetCore
{
    public class AuthorizeDirective
    {
        public AuthorizeDirective(string policy)
            : this(policy, null)
        {
        }

        public AuthorizeDirective(IEnumerable<string> roles)
            : this(null, roles)
        {
        }

        public AuthorizeDirective(string policy, IEnumerable<string> roles)
        {
            ReadOnlyCollection<string> readOnlyRoles =
                roles?.ToList().AsReadOnly();

            if (string.IsNullOrEmpty(policy)
                && (readOnlyRoles == null || readOnlyRoles.Any()))
            {
                throw new ArgumentException(
                    "Either policy or roles has to be set.");
            }

            Policy = policy;
            Roles = readOnlyRoles;
        }

        /// <summary>
        /// Gets or sets the policy name that determines access to the resource.
        /// </summary>
        public string Policy { get; }

        /// <summary>
        /// Gets or sets of roles that are allowed to access the resource.
        /// </summary>
        public IReadOnlyCollection<string> Roles { get; }
    }
}
