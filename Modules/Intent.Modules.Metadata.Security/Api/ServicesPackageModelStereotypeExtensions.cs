using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.Security.Api
{
    public static class ServicesPackageModelStereotypeExtensions
    {
        public static Secured GetSecured(this ServicesPackageModel model)
        {
            var stereotype = model.GetStereotype(Secured.DefinitionId);
            return stereotype != null ? new Secured(stereotype) : null;
        }

        public static IReadOnlyCollection<Secured> GetSecureds(this ServicesPackageModel model)
        {
            var stereotypes = model
                .GetStereotypes(Secured.DefinitionId)
                .Select(stereotype => new Secured(stereotype))
                .ToArray();

            return stereotypes;
        }


        public static bool HasSecured(this ServicesPackageModel model)
        {
            return model.HasStereotype(Secured.DefinitionId);
        }

        public static bool TryGetSecured(this ServicesPackageModel model, out Secured stereotype)
        {
            if (!HasSecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Secured(model.GetStereotype(Secured.DefinitionId));
            return true;
        }

        public static Unsecured GetUnsecured(this ServicesPackageModel model)
        {
            var stereotype = model.GetStereotype(Unsecured.DefinitionId);
            return stereotype != null ? new Unsecured(stereotype) : null;
        }


        public static bool HasUnsecured(this ServicesPackageModel model)
        {
            return model.HasStereotype(Unsecured.DefinitionId);
        }

        public static bool TryGetUnsecured(this ServicesPackageModel model, out Unsecured stereotype)
        {
            if (!HasUnsecured(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Unsecured(model.GetStereotype(Unsecured.DefinitionId));
            return true;
        }

        public class Secured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "a9eade71-1d56-4be7-a80c-81046c0c978b";

            public Secured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Roles()
            {
                return _stereotype.GetProperty<string>("Roles");
            }

            public string Policy()
            {
                return _stereotype.GetProperty<string>("Policy");
            }

            public IElement[] SecurityRoles()
            {
                return _stereotype.GetProperty<IElement[]>("Security Roles") ?? new IElement[0];
            }

            public IElement SecurityPolicies()
            {
                return _stereotype.GetProperty<IElement>("Security Policies");
            }

        }

        public class Unsecured
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8b65c29e-1448-43ac-a92a-0e0f86efd6c6";

            public Unsecured(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}