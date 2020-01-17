using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Electron.IpcProxy.Templates.CSharpReceivingProxy
{
    [Description(CSharpIpcReceivingProxyTemplate.Identifier)]
    public class Registrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => CSharpIpcReceivingProxyTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new CSharpIpcReceivingProxyTemplate(model, project);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication application)
        {
            return _metadataManager
                .GetServices(application.Id)
                .Where(x => x.Stereotypes.Any(y => y.Name == "IpcService"))
                .ToList();
        }
    }
}
