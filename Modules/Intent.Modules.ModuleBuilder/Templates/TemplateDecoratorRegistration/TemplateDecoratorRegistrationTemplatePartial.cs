using System;
using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecorator;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorRegistration
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class TemplateDecoratorRegistrationTemplate : CSharpTemplateBase<TemplateDecoratorModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.TemplateDecoratorRegistration";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public TemplateDecoratorRegistrationTemplate(IOutputTarget outputTarget, TemplateDecoratorModel model) : base(TemplateId, outputTarget, model)
        {
            AddTypeSource(TemplateDecoratorContractTemplate.TemplateId);
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Registration",
                @namespace: $"{this.GetNamespaceWithSingleOnlyOf("Decorators")}",
                relativeLocation: $"{this.GetFolderPathWithout("Decorators")}");
        }

        private string GetDecoratorTypeName()
        {
            return GetTypeName(TemplateDecoratorTemplate.TemplateId, Model);
        }

        private string GetDecoratorContractTypeName()
        {
            return GetTypeName(Model.TypeReference);
        }

        private string GetTemplateTypeName()
        {
            return ((IElement)Model.TypeReference.Element).ParentElement.Name.RemoveSuffix("Template") + "Template";
        }
    }
}