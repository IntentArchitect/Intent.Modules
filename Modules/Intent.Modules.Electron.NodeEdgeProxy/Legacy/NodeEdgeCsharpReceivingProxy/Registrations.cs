using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;

namespace Intent.Packages.Electron.NodeEdgeProxy.Legacy.NodeEdgeCsharpReceivingProxy
{
    [Description("Intent.Packages.Electron.NodeEdgeProxy - CSharp Receiving Proxies (Legacy)")]
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
                .GetMetaData<ServiceModel>(new MetaDataType("Service-Legacy"))
                .Where(x => x.ApplicationName == applicationManager.ApplicationName)
                .ToList();

        }
    }
}
