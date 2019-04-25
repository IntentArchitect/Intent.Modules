using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.Mapping
{
    [Description(MappingTemplate.IDENTIFIER)]
    public class MappingTemplateRegistrations : ModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly ServicesMetadataProvider _metaDataManager;

        public MappingTemplateRegistrations(ServicesMetadataProvider metaDataManager)
        {
            _metaDataManager = metaDataManager;
            
            FilterExpression = "model.MappedClass != null";
        }

        public override string TemplateId => MappingTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project, IDTOModel model)
        {
            return new MappingTemplate(project, model);
        }

        public override IEnumerable<IDTOModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetDTOs(application);
        }
    }
}
