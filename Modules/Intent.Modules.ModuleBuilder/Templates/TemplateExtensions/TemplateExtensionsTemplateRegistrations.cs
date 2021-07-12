using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Templates.Registration.FilePerModel;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.TemplateExtensions
{
    public class TemplateExtensionsTemplateRegistrations : SingleFileTemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public TemplateExtensionsTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => TemplateExtensionsTemplate.TemplateId;


        public override ITemplate CreateTemplateInstance(IOutputTarget project)
        {
            return new TemplateExtensionsTemplate(project);
        }
    }
}