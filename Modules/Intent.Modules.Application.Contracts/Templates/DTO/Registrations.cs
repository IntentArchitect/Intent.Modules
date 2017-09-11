using System;
using System.Linq;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Registrations;
using Intent.Packages.Application.Contracts.Templates.DTO;
using System.ComponentModel;
using Intent.Packages.Application.Contracts.Decorators;
using Intent.SoftwareFactory.Templates.Registrations;
using Intent.MetaModel.DTO;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Packages.Application.Contracts.DTO
{
    [Description("Intent Applications Contracts DTO")]
    public class Registrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;

        }

        public override string TemplateId
        {
            get
            {
                return DTOTemplate.Identifier;
            }
        }

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new DTOTemplate(project, model);
        }

        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetMetaData<Intent.MetaModel.DTO.DTOModel>(new MetaDataType("DTO")).Where(x => x.Application.Name == application.ApplicationName).ToList();
        }
    }
}

