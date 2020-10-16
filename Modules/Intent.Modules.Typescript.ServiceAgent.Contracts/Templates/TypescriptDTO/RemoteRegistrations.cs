using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.Types.Api;
using Intent.Templates;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    [Description("Intent Typescript ServiceAgent DTO - Remote")]
    public class RemoteRegistrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "TypeScript";

        public RemoteRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;

        }

        public override string TemplateId => TypescriptDtoTemplate.RemoteIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new TypescriptDtoTemplate(TemplateId, project, model);
        }

        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            var dtoModels = _metadataManager.GetSolutionMetadata<IElement>("Services")
                .Where(x => x.SpecializationType == DTOModel.SpecializationType)
                .Select(x => new DTOModel(x))
                .Where(x => GetConsumers(x).Any(y => application.Name.Equals(y, StringComparison.OrdinalIgnoreCase)))
                .ToList<DTOModel>();

            return dtoModels;
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            settings.SetIfSupplied("Consumer Stereotype Name", (s) => _stereotypeName = s);
            settings.SetIfSupplied("Consumer Stereotype Property Name", (s) => _stereotypePropertyName = s);
        }

        private IEnumerable<string> GetConsumers(DTOModel dto)
        {
            return dto.HasStereotype(_stereotypeName)
                ? dto.GetStereotypeProperty(_stereotypeName, _stereotypePropertyName, "").Split(',').Select(x => x.Trim()).ToArray()
                : dto.GetStereotypeInFolders(_stereotypeName).GetProperty(_stereotypePropertyName, "").Split(',').Select(x => x.Trim()).ToArray();
        }
    }
}
