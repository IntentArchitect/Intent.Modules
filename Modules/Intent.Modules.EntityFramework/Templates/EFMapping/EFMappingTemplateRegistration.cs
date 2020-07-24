using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Modelers.Domain;
using Intent.Templates;


namespace Intent.Modules.EntityFramework.Templates.EFMapping
{
    [Description(EFMappingTemplate.Identifier)]
    public class EFMappingTemplateRegistration : ModelTemplateRegistrationBase<ClassModel>
    {
        private readonly IMetadataManager _metadataManager;

        public EFMappingTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => EFMappingTemplate.Identifier;

        public override ITemplate CreateTemplateInstance(IProject project, ClassModel model)
        {
            return new EFMappingTemplate(model, project);
        }

        public override IEnumerable<ClassModel> GetModels(Engine.IApplication application)
        {
            return _metadataManager.Domain(application).GetClassModels().ToList();
        }
    }
}
