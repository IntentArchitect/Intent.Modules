using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.TemplateExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TemplateExtensionsTemplate : CSharpTemplateBase<object>
    {
        //protected List<IModuleBuilderTemplate> Templates = new List<IModuleBuilderTemplate>();
        protected List<IModuleBuilderTemplate> Templates = new List<IModuleBuilderTemplate>();
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.TemplateExtensions";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TemplateExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, model)
        {
            ExecutionContext.EventDispatcher.Subscribe<TemplateRegistrationRequiredEvent>(@event =>
            {
                if (@event.SourceTemplateId != null)
                {
                    var template = GetTemplate<IModuleBuilderTemplate>(@event.SourceTemplateId, @event.ModelId);
                    if (template.Model is not TemplateRegistrationModel)
                    {
                        return;
                    }

                    Templates.Add(template);
                }
            });
        }

        public override bool CanRunTemplate()
        {
            return Templates.Any();
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"TemplateExtensions",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}