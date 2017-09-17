using Intent.MetaModel.DTO;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Templates.Registrations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.Application.Contracts.Mappings.Templates.Mapping
{
    [Description("Intent Applications Contract Mapping Extentions Template")]
    public class Registrations : ModelTemplateRegistrationBase<DTOModel>
    {

        private IMetaDataManager _metaDataManager;


        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
            
            FilterExpression = "!string.IsNullOrWhiteSpace(model.MappedClassId)";
        }

        public override string TemplateId
        {
            get
            {
                return MappingTemplate.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new MappingTemplate(project, model);
        }

        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetMetaData<Intent.MetaModel.DTO.DTOModel>(new MetaDataType("DTO")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
