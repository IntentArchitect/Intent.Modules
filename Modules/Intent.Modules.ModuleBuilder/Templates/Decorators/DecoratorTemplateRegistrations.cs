using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Helpers;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Decorators
{
    public class DecoratorTemplateRegistrations : ModelTemplateRegistrationBase<IDecoratorDefinition>
    {
        public override string TemplateId => DecoratorTemplate.TemplateId;

        private readonly IMetadataManager _metadataManager;

        public DecoratorTemplateRegistrations(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override ITemplate CreateTemplateInstance(IProject project, IDecoratorDefinition model)
        {
            return new DecoratorTemplate(project, model);
        }

        public override IEnumerable<IDecoratorDefinition> GetModels(IApplication application)
        {
            return _metadataManager.GetDecorators(application)
                .ToList();
        }
    }
}
