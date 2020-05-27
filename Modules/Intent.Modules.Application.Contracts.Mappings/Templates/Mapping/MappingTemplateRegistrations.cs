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
    public class MappingTemplateRegistrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private readonly ApiMetadataProvider _metadataManager;

        public MappingTemplateRegistrations(ApiMetadataProvider metadataManager)
        {
            _metadataManager = metadataManager;

            FilterExpression = "model.Mapping != null";
        }

        public override string TemplateId => MappingTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new MappingTemplate(project, model);
        }

        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            return _metadataManager.GetDTOModels(application);
        }
    }
}
