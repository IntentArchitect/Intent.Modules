using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Electron.IpcProxy.Templates.AngularTypeScriptIpcServiceProxy
{
    [Description(AngularTypeScriptIpcServiceProxyTemplate.Identifier)]
    public class Registrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public Registrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularTypeScriptIpcServiceProxyTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new AngularTypeScriptIpcServiceProxyTemplate(model, project);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication applicationManager)
        {
            return _metaDataManager
                .GetServices(applicationManager)
                .Where(x => x.Stereotypes.Any(y => y.Name == "IpcService"))
                .ToList();
        }
    }
}
