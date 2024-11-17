#nullable enable
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Metadata.Security.Models
{
    internal class SecurityModel : ISecurityModel
    {
        public SecurityModel(IReadOnlyCollection<string> roles, IReadOnlyCollection<string> policies)
        {
            EquatableRoles = string.Join(",", roles.Order());
            EquatablePolicies = string.Join(",", policies.Order());
            Roles = roles;
            Policies = policies;
        }

        public string EquatableRoles { get; }
        public string EquatablePolicies { get; }
        public IReadOnlyCollection<string> Roles { get; }
        public IReadOnlyCollection<string> Policies { get; }
    }
}
