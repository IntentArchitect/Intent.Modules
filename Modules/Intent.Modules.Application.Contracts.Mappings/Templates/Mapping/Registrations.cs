using System;
using System.Linq;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.Packages.Application.Contracts.Mappings.Templates.Mapping;
using System.ComponentModel;
using Intent.MetaModel.DTO;
using Intent.SoftwareFactory.Templates.Registrations;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Packages.Application.Contracts.Mapping
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
