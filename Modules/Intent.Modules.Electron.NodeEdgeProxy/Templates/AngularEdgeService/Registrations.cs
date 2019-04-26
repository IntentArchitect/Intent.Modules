using Intent.Engine;
using Intent.Templates;

using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularEdgeService
{
    [Description(AngularEdgeServiceProviderTemplate.Identifier)]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        private readonly IMetadataManager _metaDataManager;

        public Registrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }
        
        public override string TemplateId => AngularEdgeServiceProviderTemplate.Identifier;
        
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            //var hostingConfig = _metaDataManager.GetMetaData<HostingConfigModel>("LocalHosting").SingleOrDefault(x => x.ApplicationName == project.ApplicationName());
            
            return new AngularEdgeServiceProviderTemplate(project, project.Application.EventDispatcher);
        }
    }
}
