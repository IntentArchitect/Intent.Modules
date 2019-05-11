using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO
{
    [Description("Intent Typescript ServiceAgent DTO - Remote")]
    public class RemoteRegistrations : ModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly IMetadataManager _metaDataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "CommaSeperatedList";

        public RemoteRegistrations(IMetadataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;

        }

        public override string TemplateId => TypescriptDtoTemplate.RemoteIdentifier;

        public override ITemplate CreateTemplateInstance(IProject project, IDTOModel model)
        {
            return new TypescriptDtoTemplate(TemplateId, project, model);
        }

        public override IEnumerable<IDTOModel> GetModels(IApplication application)
        {
            var dtoModels = _metaDataManager.GetMetaData<IDTOModel>("Services").ToArray();
            if (!dtoModels.Any())
            {
                dtoModels = _metaDataManager.GetMetaData<IDTOModel>("DTO").ToArray(); // backward compatibility
            }

            dtoModels = dtoModels
                .Where(x => GetConsumers(x).Any(y => application.ApplicationName.Equals(y, StringComparison.OrdinalIgnoreCase)))
                .ToArray();

            return dtoModels.ToList();
        }

        public override void Configure(IDictionary<string, string> settings)
        {
            base.Configure(settings);
            settings.SetIfSupplied("Consumer Stereotype Name", (s) => _stereotypeName = s);
            settings.SetIfSupplied("Consumer Stereotype Property Name", (s) => _stereotypePropertyName = s);
        }

        private IEnumerable<string> GetConsumers(IDTOModel dto)
        {
            return dto.HasStereotype(_stereotypeName)
                ? dto.GetStereotypeProperty(_stereotypeName, _stereotypePropertyName, "").Split(',').Select(x => x.Trim()).ToArray()
                : dto.GetStereotypeInFolders(_stereotypeName).GetProperty(_stereotypePropertyName, "").Split(',').Select(x => x.Trim()).ToArray();
        }
    }
}
