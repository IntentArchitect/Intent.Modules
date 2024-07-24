using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiMetadataProviderExtensions", Version = "1.0")]

namespace Intent.Metadata.WebApi.Api
{
    public static class ApiMetadataProviderExtensions
    {
        public static IList<ApiVersionModel> GetApiVersionModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(ApiVersionModel.SpecializationTypeId)
                .Select(x => new ApiVersionModel(x))
                .ToList();
        }

        public static IList<PolicyModel> GetPolicyModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(PolicyModel.SpecializationTypeId)
                .Select(x => new PolicyModel(x))
                .ToList();
        }

        public static IList<RoleModel> GetRoleModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(RoleModel.SpecializationTypeId)
                .Select(x => new RoleModel(x))
                .ToList();
        }

        public static IList<SecurityConfigurationModel> GetSecurityConfigurationModels(this IDesigner designer)
        {
            return designer.GetElementsOfType(SecurityConfigurationModel.SpecializationTypeId)
                .Select(x => new SecurityConfigurationModel(x))
                .ToList();
        }

    }
}