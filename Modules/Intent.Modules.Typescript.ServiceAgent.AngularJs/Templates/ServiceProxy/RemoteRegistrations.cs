using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.Common.Types.Api;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    [Description("Intent Typescript ServiceAgent Proxy - Other Servers")]
    public class RemoteRegistrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetadataManager _metadataManager;

        public RemoteRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new TypescriptWebApiClientServiceProxyTemplate(TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier, project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metadataManager.GetSolutionMetadata<IElement>("Services")
                .Where(x => x.SpecializationType == ServiceModel.SpecializationType)
                .Select(x => new ServiceModel(x))
                .ToList();

            serviceModels = serviceModels
                .Where(x => GetConsumers(x).Any(y => y.Equals(application.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return serviceModels.ToList();
        }

        public static IEnumerable<string> GetConsumers<T>(T item) where T : IHasFolder, IHasStereotypes
        {
            if (item.HasStereotype("Consumers"))
            {
                return item
                    .GetStereotypeProperty(
                        "Consumers",
                        "CommaSeperatedList",
                        item.GetStereotypeProperty(
                            "Consumers",
                            "TypeScript",
                            ""))
                    .Split(',')
                    .Select(x => x.Trim());
            }

            return item
                .GetStereotypeInFolders("Consumers")
                .GetProperty(
                    "CommaSeperatedList",
                    item
                        .GetStereotypeInFolders("Consumers")
                        .GetProperty("TypeScript", ""))
                .Split(',')
                .Select(x => x.Trim());
        }
    }
}
