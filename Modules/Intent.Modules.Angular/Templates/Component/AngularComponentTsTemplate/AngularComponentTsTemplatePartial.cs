using System;
using System.IO;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.AngularModuleTemplate;
using Intent.Modules.Angular.Templates.Proxies.AngularDTOTemplate;
using Intent.Modules.Common.Plugins;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularComponentTsTemplate : IntentTypescriptProjectItemTemplateBase<IComponentModel>, ITemplateBeforeExecutionHook
    {
        public const string TemplateId = "Angular.AngularComponentTsTemplate";

        public AngularComponentTsTemplate(IProject project, IComponentModel model) : base(TemplateId, project, model)
        {
        }

        public string ComponentName
        {
            get
            {
                if (Model.Name.EndsWith("Component", StringComparison.InvariantCultureIgnoreCase))
                {
                    return Model.Name.Substring(0, Model.Name.Length - "Component".Length);
                }
                return Model.Name;
            }
        }

        public string ModuleName { get; private set; }

        public void BeforeTemplateExecution()
        {
            Types.AddClassTypeSource(TypescriptTypeSource.InProject(Project, AngularDTOTemplate.TemplateId, "{0}[]"));

            if (File.Exists(GetMetadata().GetFullLocationPathWithFileName()))
            {
                return;
            }

            // New Component:
            Project.Application.EventDispatcher.Publish(AngularComponentCreatedEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularComponentCreatedEvent.ModuleId, Model.Module.Id },
                    {AngularComponentCreatedEvent.ModelId, Model.Id},
                });
        }

        public override string RunTemplate()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);

            var editor = new TypescriptFileEditor(source);
           
            foreach (var model in Model.Models)
            {
                if (!editor.NodeExists($"PropertyDeclaration:{model.Name}"))
                {
                    editor.AddProperty($@"
  {model.Name}: {Types.Get(model.Type)};");
                }
            }

            foreach (var operation in Model.Commands)
            {
                if (!editor.MethodExists(operation.Name.ToCamelCase()))
                {
                    editor.AddMethod($@"

  {operation.Name.ToCamelCase()}({string.Join(", ", operation.Parameters.Select(x => x.Name.ToCamelCase() + (x.Type.IsNullable ? "?" : "") + ": " + Types.Get(x.Type, "{0}[]")))}): {(operation.ReturnType != null ? Types.Get(operation.ReturnType.Type) : "void")} {{

  }}");
                }
            }

            var dependencies = Types.GetTemplateDependencies().Select(x => Project.FindTemplateInstance<ITemplate>(x));
            foreach (var template in dependencies)
            {
                if (!(template is IHasClassDetails))
                {
                    continue;
                }

                editor.AddImportIfNotExists(((IHasClassDetails)template).ClassName, GetMetadata().GetRelativeFilePathWithFileName().GetRelativePath(template.GetMetadata().GetRelativeFilePathWithFileName())); // Temporary replacement until 1.9 changes are merged.
            }

            return editor.GetSource();
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : TransformText();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override TypescriptDefaultFileMetadata DefineTypescriptDefaultFileMetadata()
        {
            var moduleTemplate = Project.FindTemplateInstance<AngularModuleTemplate.AngularModuleTemplate>(AngularModuleTemplate.AngularModuleTemplate.TemplateId, Model.Module);
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{ComponentName.ToAngularFileName()}.component",
                fileExtension: "ts", 
                defaultLocationInProject: $"Client\\src\\app\\{moduleTemplate.ModuleName.ToAngularFileName()}\\{ComponentName.ToAngularFileName()}",
                className: "${ComponentName}Component"
            );
        }
    }
}