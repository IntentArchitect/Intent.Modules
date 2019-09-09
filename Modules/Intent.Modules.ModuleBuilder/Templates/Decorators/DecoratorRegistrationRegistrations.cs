using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Decorators
{
    public class DecoratorRegistrationRegistrations : ModelTemplateRegistrationBase<IDecoratorDefinition>
    {
        public override string TemplateId => DecoratorRegistrationTemplate.TemplateId;

        private readonly IMetadataManager _metadataManager;

        public DecoratorRegistrationRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ITemplate CreateTemplateInstance(IProject project, IDecoratorDefinition model)
        {
            return new DecoratorRegistrationTemplate(project, model);
        }

        public override IEnumerable<IDecoratorDefinition> GetModels(IApplication applicationManager)
        {
            return _metadataManager.GetDecorators(applicationManager)
                .ToList();
        }
    }
}
