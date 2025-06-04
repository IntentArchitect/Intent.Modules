#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Intent.Exceptions;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.Metadata.Security.Models;

public static class SecurityModelHelpers
{
    public static IEnumerable<ISecurityModel> GetSecurityModels(IElement element, bool checkParents = true)
    {
        return GetSecurityModels(hasStereotypes: element, checkParents);
    }

    public static IEnumerable<ISecurityModel> GetSecurityModels(IPackage package, bool checkParents = true)
    {
        return GetSecurityModels(hasStereotypes: package, checkParents);
    }

    private static IEnumerable<ISecurityModel> GetSecurityModels(IHasStereotypes hasStereotypes, bool checkParents)
    {
        while (true)
        {
            var hasAuthorization = false;

            foreach (var stereotype in hasStereotypes.GetStereotypes(Constants.Stereotypes.Secured.Id))
            {
                hasAuthorization = true;
                var commaSeparatedRoles = stereotype.GetProperty<string?>(Constants.Stereotypes.Secured.Properties.CommaSeparatedRoles);
                var textPolicy = stereotype.GetProperty<string?>(Constants.Stereotypes.Secured.Properties.TextPolicy);
                var roles = stereotype.GetProperty<IElement[]?>(Constants.Stereotypes.Secured.Properties.Roles);
                var policy = stereotype.GetProperty<IElement?>(Constants.Stereotypes.Secured.Properties.Policies);

                // Based on looking at old controller logic, this convention for plus signs to
                // indicate ANDing the requirements never seemed to apply to policies.
                if (commaSeparatedRoles?.Contains('+') == true)
                {
                    var roleGroups = commaSeparatedRoles
                        .Split('+')
                        .Where(x => !string.IsNullOrWhiteSpace(x));

                    foreach (var roleGroup in roleGroups)
                    {
                        yield return new SecurityModel(roles: Split(roleGroup), policies: []);
                    }

                    continue;
                }

                yield return new SecurityModel(
                    roles: (roles?.Length ?? 0) > 0
                        ? roles!.Select(x => x.Name).ToArray()
                        : Split(commaSeparatedRoles),
                    policies: !string.IsNullOrWhiteSpace(policy?.Name) ? [policy.Name] : string.IsNullOrWhiteSpace(textPolicy) ? [] : [textPolicy]);

            }

            var element = hasStereotypes as IElement;

            if (hasStereotypes.HasStereotype(Constants.Stereotypes.Unsecured.Id))
            {
                if (!hasAuthorization)
                {
                    yield break;
                }

                if (element != null)
                {
                    throw new ElementException(element, "Cannot require authorization and allow-anonymous at the same time");
                }

                if (hasStereotypes is IPackage package)
                {
                    throw new Exception($"{package.Name} [{package.Id}] cannot require authorization and allow-anonymous at the same time");
                }

                throw new Exception("Cannot require authorization and allow-anonymous at the same time");
            }

            if (!checkParents || element == null)
            {
                yield break;
            }

            hasStereotypes = element.ParentElement != null
                ? element.ParentElement
                : element.Package;
        }

        static string[] Split(string? value)
        {
            if (value == null)
            {
                return [];
            }

            return value
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }
    }
}