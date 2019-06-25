using System.Collections.Generic;
using System.ComponentModel;
using Intent.Engine;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common.Registrations;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.Mapping
{
    [Description(MappingTemplate.Identifier)]
    public class MappingTemplateRegistrations : ModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly ServicesMetadataProvider _metadataManager;

        public MappingTemplateRegistrations(ServicesMetadataProvider metadataManager)
        {
            _metadataManager = metadataManager;

            FilterExpression = "model.MappedClass != null";
        }

        public override string TemplateId => MappingTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IDTOModel model)
        {
            return new MappingTemplate(project, model);
        }

        public override IEnumerable<IDTOModel> GetModels(IApplication application)
        {
            return _metadataManager.GetDTOs(application);
        }
    }
}
