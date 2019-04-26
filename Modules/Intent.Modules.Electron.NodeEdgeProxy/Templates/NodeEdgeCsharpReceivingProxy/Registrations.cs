using Intent.Modelers.Services.Api;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy
{
    [Description("Intent.Packages.Electron.NodeEdgeProxy - CSharp Receiving Proxies")]
    public class Registrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetadataManager _metaDataManager;

        public Registrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => NodeEdgeCsharpReceivingProxyTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new NodeEdgeCsharpReceivingProxyTemplate(model, project);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication applicationManager)
        {
            return _metaDataManager
                .GetServices(applicationManager)
                .Where(x => x.Stereotypes.Any(y => y.Name == "NodeEdgeService"))
                .ToList();
        }
    }
}
