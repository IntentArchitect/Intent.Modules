using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Helpers;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.FileTemplateT4
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class FileTemplateT4Template : IntentTemplateBase<FileTemplateModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.FileTemplateT4";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public FileTemplateT4Template(IOutputTarget outputTarget, FileTemplateModel model) : base(TemplateId, outputTarget, model)
        {
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{TemplateName}",
                fileExtension: "tt",
                relativeLocation: $"{this.GetFolderPath(additionalFolders: Model.Name.ToCSharpIdentifier().RemoveSuffix("Template"))}");
        }


        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public override string TransformText()
        {
            if (TryGetExistingFileContent(out var content))
            {
                return TemplateHelper.ReplaceTemplateInheritsTag(content, $"{GetBaseType()}");
            }

            return $@"<#@ template language=""C#"" inherits=""{GetBaseType()}"" #>
<#@ assembly name=""System.Core"" #>
<#@ import namespace=""System.Collections.Generic"" #>
<#@ import namespace=""System.Linq"" #>
<#@ import namespace=""Intent.Modules.Common"" #>
<#@ import namespace=""Intent.Modules.Common.Templates"" #>
<#@ import namespace=""Intent.Metadata.Models"" #>
{(Model.GetModelType() != null ? $@"<#@ import namespace=""{Model.GetModelType()?.ParentModule.ApiNamespace}"" #>" : "")}

// Place your file template logic here
";
        }

        private string GetBaseType()
        {
            if (Model.DecoratorContract != null)
            {
                return $"{GetTemplateBaseClass()}<{Model.GetModelName()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>";
            }
            return $"{GetTemplateBaseClass()}<{Model.GetModelName()}>";
        }

        private string GetTemplateBaseClass()
        {
            return nameof(IntentTemplateBase);
        }
    }

}