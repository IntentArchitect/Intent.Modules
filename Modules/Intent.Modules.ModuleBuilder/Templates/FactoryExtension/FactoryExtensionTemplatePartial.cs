using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.FactoryExtension
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class FactoryExtensionTemplate : CSharpTemplateBase<FactoryExtensionModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.FactoryExtension";

        public FactoryExtensionTemplate(IOutputTarget outputTarget, FactoryExtensionModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public override void BeforeTemplateExecution()
        {
            ExecutionContext.EventDispatcher.Publish(new FactoryExtensionRegistrationRequiredEvent(Model.Id, GetId()));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetId()
        {
            return $"{Project.ApplicationName().ToCSharpNamespace()}.{Model.Name.ToCSharpIdentifier()}";
        }
    }
}