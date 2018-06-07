using Intent.MetaModel.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.MappingProfile
{
    [Description("Intent Applications Contract Mapping Profile Template")]
    public class MappingProfileTemplateRegistrations : ListModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;


        public MappingProfileTemplateRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
            
            FilterExpression = "!string.IsNullOrWhiteSpace(model.MappedClassId)";
        }

        public override string TemplateId => MappingProfileTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IList<IDTOModel> models)
        {
            return new MappingProfileTemplate(project, models);
        }

        public override IList<IDTOModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetDTOModels(application).ToList();
        }
    }
}
