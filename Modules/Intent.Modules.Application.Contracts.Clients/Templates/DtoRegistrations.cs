using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.MetaModel.DTO;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Application.Contracts.Clients.Templates
{
    [Description("Intent Applications Contracts DTO")]
    public class DtoRegistrations : ModelTemplateRegistrationBase<IDTOModel>
    {
        private readonly IMetaDataManager _metaDataManager;

        private string _stereotypeName = "Consumers";
        private string _stereotypePropertyName = "CSharp";

        public DtoRegistrations(IMetaDataManager metaDataManager)
        {
            _metaDataManager = metaDataManager;
        }

        public override string TemplateId => TemplateIds.ClientDto;

        public override ITemplate CreateTemplateInstance(IProject project, IDTOModel model)
        {
            return new DTOTemplate(project, model, TemplateId);
        }

        public override IEnumerable<IDTOModel> GetModels(IApplication application)
        {
            var dtoModels = _metaDataManager.GetMetaData<IDTOModel>("Services").ToArray();
            if (!dtoModels.Any())
            {
                dtoModels = _metaDataManager.GetMetaData<IDTOModel>("DTO").ToArray(); // backward compatibility
            }

            return dtoModels
                .Where(x => x.GetConsumers(_stereotypeName, _stereotypePropertyName).Any(y => y.Equals(application.ApplicationName, StringComparison.OrdinalIgnoreCase)))
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
