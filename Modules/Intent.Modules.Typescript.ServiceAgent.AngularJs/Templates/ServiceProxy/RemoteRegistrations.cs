using System;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using IApplication = Intent.SoftwareFactory.Engine.IApplication;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    [Description("Intent Typescript ServiceAgent Proxy - Other Servers")]
    public class RemoteRegistrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public RemoteRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new TypescriptWebApiClientServiceProxyTemplate(TypescriptWebApiClientServiceProxyTemplate.RemoteIdentifier, project, model, project.Application.EventDispatcher);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            var serviceModels = _metaDataManager.GetMetaData<IServiceModel>("Services");

            serviceModels = serviceModels
                .Where(x => GetConsumers(x).Any(y => y.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
                .ToList();


            return serviceModels.ToList();
        }

        public static IEnumerable<string> GetConsumers<T>(T item) where T: IHasFolder, IHasStereotypes
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
