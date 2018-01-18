using Intent.MetaModel.Service;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.AngularEdgeService
{
    [Description("Intent.Packages.Electron.NodeEdgeProxy - Angular Service")]
    public class Registrations : NoModelTemplateRegistrationBase
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => AngularEdgeServiceTemplate.Identifier;
        public override ITemplate CreateTemplateInstance(IProject project)
        {
            return new AngularEdgeServiceTemplate(project);
        }
    }
}
