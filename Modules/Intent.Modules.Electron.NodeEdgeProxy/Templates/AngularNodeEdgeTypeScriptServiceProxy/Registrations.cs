using Intent.MetaModel.Hosting;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularNodeEdgeTypeScriptServiceProxy
{
    [Description("Intent.Packages.Electron.NodeEdgeProxy - Angular TypeScript Proxies")]
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
            var hostingConfig = _metaDataManager.GetMetaData<HostingConfigModel>("LocalHosting").SingleOrDefault(x => x.ApplicationName == project.ApplicationName());

            return new AngularNodeEdgeTypeScriptServiceProxyTemplate(
                model: model,
                hostingConfig: hostingConfig,
                project: project,
                eventDispatcher: project.Application.EventDispatcher);
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
