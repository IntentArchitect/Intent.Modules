using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common.FileBuilders.IndentedFileBuilder;
using Intent.Modules.Common.FileBuilders.DataFileBuilder;
using Intent.Modules.Common.FileBuilders.DataFileBuilder.Writers;
using Intent.Modules.ModuleBuilder.Templates.IModSpec;
using Intent.Modules.ModuleBuilder.Templates.TemplateDecoratorContract;
using TemplatingMethod = Intent.ModuleBuilder.Api.FileTemplateModelStereotypeExtensions.FileSettings.TemplatingMethodOptionsEnum;
using DataFileType = Intent.ModuleBuilder.Api.FileTemplateModelStereotypeExtensions.FileSettings.DataFileOutputTypeOptionsEnum;

namespace Intent.Modules.ModuleBuilder.Templates.FileTemplatePartial
{
    partial class FileTemplatePartialTemplate : CSharpTemplateBase<FileTemplateModel>, IHasTemplateDependencies
    {
        public const string TemplateId = "Intent.ModuleBuilder.ProjectItemTemplate.Partial";

        public FileTemplatePartialTemplate(string templateId, IOutputTarget project, FileTemplateModel model) : base(templateId, project, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);
        }

        public string TemplateName => $"{Model.Name.ToCSharpIdentifier().RemoveSuffix("Template")}Template";
        public IList<string> OutputFolders => Model.GetParentFolders().Select(x => x.Name).Concat(new[] { Model.Name.ToCSharpIdentifier().RemoveSuffix("Template") }).ToList();
        public string FolderPath => string.Join("/", OutputFolders);
        public string FolderNamespace => string.Join(".", OutputFolders);

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{TemplateName}",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                fileName: $"{TemplateName}Partial",
                relativeLocation: $"{FolderPath}");
        }

        public string GetTemplateId()
        {
            return $"{Model.GetModule().Name}.{string.Join(".", Model.GetParentFolderNames().Concat(new[] { Model.Name }))}";
        }

        public override void BeforeTemplateExecution()
        {
            Project.Application.EventDispatcher.Publish(new TemplateRegistrationRequiredEvent(
                modelId: Model.Id,
                templateId: GetTemplateId(),
                templateType: "File Template",
                role: GetRole(),
                location: Model.GetLocation()));

            if (Model.GetModelType() != null)
            {
                Project.Application.EventDispatcher.Publish(new ModuleDependencyRequiredEvent(
                    moduleId: Model.GetModelType().ParentModule.Name,
                    moduleVersion: Model.GetModelType().ParentModule.Version));
            }
        }

        private string GetRole()
        {
            return Model.GetRole();
        }

        private string GetBaseType()
        {
            var @interface = Model.GetFileSettings().TemplatingMethod().AsEnum() switch
            {
                TemplatingMethod.DataFileBuilder => $", {UseType(typeof(IDataFileBuilderTemplate).FullName)}",
                TemplatingMethod.IndentedFileBuilder => $", {UseType(typeof(IIndentedFileBuilderTemplate).FullName)}",
                _ => string.Empty
            };

            if (Model.DecoratorContract != null)
            {
                return $"{GetTemplateBaseClass()}<{GetModelType()}, {GetTypeName(TemplateDecoratorContractTemplate.TemplateId, Model.DecoratorContract)}>{@interface}";
            }
            return $"{GetTemplateBaseClass()}<{GetModelType()}>{@interface}";
        }

        private bool IsFileBuilder => Model.GetFileSettings().TemplatingMethod().AsEnum()
            is TemplatingMethod.DataFileBuilder
            or TemplatingMethod.IndentedFileBuilder;

        private string GetBuilderType() => Model.GetFileSettings().TemplatingMethod().AsEnum() switch
        {
            TemplatingMethod.DataFileBuilder => UseType(typeof(DataFile).FullName),
            TemplatingMethod.IndentedFileBuilder => UseType(typeof(IndentedFile).FullName),
            _ => throw new InvalidOperationException($"Unknown type: {Model.GetFileSettings().TemplatingMethod().AsEnum()}")
        };

        private string GetFilePropertyName() => Model.GetFileSettings().TemplatingMethod().AsEnum() switch
        {
            TemplatingMethod.DataFileBuilder => "DataFile",
            TemplatingMethod.IndentedFileBuilder => "IndentedFile",
            _ => throw new InvalidOperationException($"Unknown type: {Model.GetFileSettings().TemplatingMethod().AsEnum()}")
        };

        private string GetModelType()
        {
            return NormalizeNamespace(Model.GetModelName());
        }

        private string GetTemplateBaseClass()
        {
            return Model.GetFileSettings().OutputFileContent().IsBinary()
                ? nameof(IntentBinaryTemplateBase)
                : nameof(IntentTemplateBase);
        }

        private string GetWriterType()
        {
            var dataFileType = Model.GetFileSettings().DataFileOutputType().AsEnum();

            return dataFileType switch
            {
                DataFileType.JSON => ".WithJsonWriter()",
                DataFileType.OCL => ".WithOclWriter()",
                DataFileType.YAML => ".WithYamlWriter()",
                DataFileType.Custom => $".WithWriter(() => new {UseType(typeof(DataFileWriter).FullName)}(), \"{Model.GetFileSettings().FileExtension()}\") // Replace with your own specialization",
                _ => throw new InvalidOperationException($"Unknown type: {dataFileType}")
            };
        }

        private string GetDefaultName()
        {
            return Model.IsFilePerModelTemplateRegistration()
                ? "{Model.Name}"
                : Model.Name.Replace("Template", "");
        }
    }
}
