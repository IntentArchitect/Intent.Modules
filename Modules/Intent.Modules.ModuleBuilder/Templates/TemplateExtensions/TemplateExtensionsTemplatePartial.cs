using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]
[assembly: DefaultIntentManaged(Mode.Merge)]

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
        public TemplateExtensionsTemplate(IOutputTarget outputTarget, object model = null) : base(TemplateId, outputTarget, null)
        {
            ExecutionContext.EventDispatcher.Subscribe<TemplateRegistrationRequiredEvent>(@event =>
            {
                //var template = @event.ModelId != null
                //    ? GetTemplate<IModuleBuilderTemplate>(@event.TemplateId, @event.ModelId, new TemplateDiscoveryOptions() { ThrowIfNotFound = false })
                //    : GetTemplate<IModuleBuilderTemplate>(@event.TemplateId, new TemplateDiscoveryOptions() { ThrowIfNotFound = false });
                //if (template != null)
                //{
                //    Templates.Add(template);
                //}
                if (@event.SourceTemplateId != null)
                {
                    Templates.Add(this.GetTemplate<IModuleBuilderTemplate>(@event.SourceTemplateId, @event.ModelId));
                }
            });
        }

        public override bool CanRunTemplate()
        {
            return Templates.Any();
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"TemplateExtensions",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}