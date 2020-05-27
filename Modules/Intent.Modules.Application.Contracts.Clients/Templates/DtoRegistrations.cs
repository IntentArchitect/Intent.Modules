using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    [Description("Intent Applications Contracts DTO")]
    public class DtoRegistrations : ModelTemplateRegistrationBase<DTOModel>
    {
        private readonly IMetadataManager _metadataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "CSharp";

        public DtoRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TemplateIds.ClientDto;

        public override ITemplate CreateTemplateInstance(IProject project, DTOModel model)
        {
            return new DTOTemplate(project, model, TemplateId);
        }

        public override IEnumerable<DTOModel> GetModels(IApplication application)
        {
            var dtoModels = _metadataManager.GetSolutionMetadata<IElement>("Services")
                .Where(x => x.SpecializationType == DTOModel.SpecializationType)
                .Select(x => new DTOModel(x))
                .ToList();

            return dtoModels
                .Where(x => x.GetConsumers(_stereotypeName, _stereotypePropertyName).Any(y => y.Equals(application.Name, StringComparison.OrdinalIgnoreCase)))
                .ToArray();
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            settings.SetIfSupplied("Consumer Stereotype Name", (s) => _stereotypeName = s);
            settings.SetIfSupplied("Consumer Stereotype Property Name", (s) => _stereotypePropertyName = s);
        }
    }
}
