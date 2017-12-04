using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy
{
    [Description("Intent.Packages.Electron.NodeEdgeProxy - CSharp Receiving Proxies")]
    public class Registrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => NodeEdgeCsharpReceivingProxyTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new NodeEdgeCsharpReceivingProxyTemplate(model, project);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication applicationManager)
        {
            return _metaDataManager
                .GetMetaData<ServiceModel>(new MetaDataIdentifier("Service"))
                .Where(x => x.Application.Name == applicationManager.ApplicationName && x.Stereotypes.Any(y => y.Name == "NodeEdgeService"))
                .ToList();

        }
    }
}
