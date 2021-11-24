using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.TemplateDecorator
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TemplateDecoratorTemplate : CSharpTemplateBase<TemplateDecoratorModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.TemplateDecorator";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TemplateDecoratorTemplate(IOutputTarget outputTarget, TemplateDecoratorModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateDecoratorContractTemplate.TemplateId);
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(new DecoratorRegistrationRequiredEvent(
                modelId: Model.Id,
                decoratorId: GetDecoratorId()));
        }

        private string GetDecoratorId()
        {
            return $"{Model.GetModule().Name}.{Model.Name}";
        }

        private string GetTemplateTypeName()
        {
            return ((IElement)Model.TypeReference.Element).ParentElement.Name.RemoveSuffix("Template") + "Template";
        }
    }
}