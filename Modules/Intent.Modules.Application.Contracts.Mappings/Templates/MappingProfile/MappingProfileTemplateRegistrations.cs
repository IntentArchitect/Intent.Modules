using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.MappingProfile
{
    [Description("Intent Applications Contract Mapping Profile Template")]
    public class MappingProfileTemplateRegistrations : ListModelTemplateRegistrationBase<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public MappingProfileTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;

            FilterExpression = "model.Mapping != null";
        }

        public override string TemplateId => MappingProfileTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project, IList<DTOModel> models)
        {
            return new MappingProfileTemplate(project, models);
        }

        public override IList<DTOModel> GetModels(IApplication application)
        {
            return _metadataManager.Services(application).GetDTOModels().ToList();
        }
    }
}
