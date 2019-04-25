using Intent.MetaModel.Hosting;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularNodeEdgeTypeScriptServiceProxy
{
    [Description(AngularNodeEdgeTypeScriptServiceProxyTemplate.Identifier)]
    public class Registrations : ModelTemplateRegistrationBase<IServiceModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularNodeEdgeTypeScriptServiceProxyTemplate.Identifier;
        
        public override ITemplate CreateTemplateInstance(IProject project, IServiceModel model)
        {
            return new AngularNodeEdgeTypeScriptServiceProxyTemplate(
                model: model,
                project: project);
        }

        public override IEnumerable<IServiceModel> GetModels(IApplication applicationManager)
        {
            return _metaDataManager
                .GetServiceModels(applicationManager)
                .Where(x => x.Stereotypes.Any(y => y.Name == "NodeEdgeService"))
                .ToList();
        }
    }
}
