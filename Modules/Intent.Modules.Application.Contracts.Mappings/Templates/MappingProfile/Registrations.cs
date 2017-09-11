
using System;
using System.Linq;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using System.ComponentModel;
using Intent.MetaModel.DTO;
using Intent.SoftwareFactory.Templates.Registrations;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using Intent.Packages.Application.Contracts.Mappings.Templates.MappingProfile;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Packages.Application.Contracts.MappingProfile
{
    [Description("Intent Applications Contract Mapping Profile Template")]
    public class Registrations : ListModelTemplateRegistrationBase<DTOModel>
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
                return MappingProfileTemplate.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project, IList<DTOModel> models)
        {
            return new MappingProfileTemplate(project, models);
        }

        public override IList<DTOModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetMetaData<Intent.MetaModel.DTO.DTOModel>(new MetaDataType("DTO")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}
