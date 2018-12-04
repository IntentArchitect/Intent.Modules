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

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    [Description(DTOTemplate.IDENTIFIER)]
    public class Registrations : ModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        public Registrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;

        }

        public override string TemplateId => DTOTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project, IDTOModel model)
        {
            return new DTOTemplate(project, model);
        }

        public override IEnumerable<IDTOModel> GetModels(IApplication application)
        {
            return _metaDataManager.GetDTOModels(application);
        }
    }
}

