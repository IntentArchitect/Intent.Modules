using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Templates;

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    [Description(DTOTemplate.IDENTIFIER)]
    public class Registrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        public Registrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;

        }

        public override string TemplateId => DTOTemplate.IDENTIFIER;

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new DTOTemplate(project, model);
        }

        public override IEnumerable<DTOModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.Services(application).GetDTOModels();
        }
    }
}

