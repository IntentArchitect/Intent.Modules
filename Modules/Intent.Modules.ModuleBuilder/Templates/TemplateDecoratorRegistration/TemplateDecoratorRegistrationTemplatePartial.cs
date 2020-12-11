using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecorator;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorRegistration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TemplateDecoratorRegistrationTemplate : CSharpTemplateBase<TemplateDecoratorModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "ModuleBuilder.Templates.TemplateDecoratorRegistration";

        public TemplateDecoratorRegistrationTemplate(IOutputTarget outputTarget, TemplateDecoratorModel model) 
            : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Registration",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        private string GetDecoratorTypeName()
        {
            return GetTypeName(TemplateDecoratorTemplate.TemplateId, Model);
        }

        private string GetTemplateTypeName()
        {
            return ((IElement) Model.TypeReference.Element).ParentElement.Name.RemoveSuffix("Template") + "Template";
        }

        private string GetDecoratorContractTypeName()
        {
            return Model.TypeReference.Element.Name;
        }
    }
}