#nullable enable
using System;
using System.Collections.Generic;

namespace Intent.Modules.Metadata.Security.Models;

public interface ISecurityModel : IEquatable<ISecurityModel>
{
    IReadOnlyCollection<string> Roles { get; }
    IReadOnlyCollection<string> Policies { get; }
    public string EquatableRoles { get; }
    public string EquatablePolicies { get; }
    public static ISecurityModel Empty { get; } = new SecurityModel([], []);

    bool IEquatable<ISecurityModel>.Equals(ISecurityModel? other)
    {
        return EquatableRoles.Equals(other?.EquatableRoles) &&
               EquatablePolicies.Equals(other.EquatablePolicies);
    }
}