using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Electron.NodeEdgeProxy.Legacy.AngularNodeEdgeTypeScriptServiceProxy
{
    [Description("Intent.Packages.Electron.NodeEdgeProxy - Angular TypeScript Proxies (Legacy)")]
    public class Registrations : ModelTemplateRegistrationBase<ServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularNodeEdgeTypeScriptServiceProxyTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ServiceModel model)
        {
            return new AngularNodeEdgeTypeScriptServiceProxyTemplate(
                model: model,
                project: project,
                applicationEvents: project.Application.EventDispatcher);
        }

        public override IEnumerable<ServiceModel> GetModels(IApplication applicationManager)
        {
            return _metaDataManager
                .GetMetaData<ServiceModel>(new MetaDataIdentifier("Service-Legacy"))
                .Where(x => x.ApplicationName == applicationManager.ApplicationName)
                .ToList();
        }
    }
}
