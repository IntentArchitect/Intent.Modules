using Intent.MetaModel.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.Mapping
{
    [Description(MappingTemplate.Identifier)]
    public class MappingTemplateRegistrations : ModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public MappingTemplateRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
            
            FilterExpression = "!string.IsNullOrWhiteSpace(model.MappedClassId)";
        }

        public override string TemplateId => MappingTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, IDTOModel model)
        {
            return new MappingTemplate(project, model);
        }

        public override IEnumerable<IDTOModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetDTOModels(application);
        }
    }
}
