using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularServiceProxyTemplate : IntentTypescriptProjectItemTemplateBase<IServiceProxyModel>, IBeforeTemplateExecutionHook
    {
        public const string TemplateId = "Angular.AngularServiceProxyTemplate";

        public AngularServiceProxyTemplate(IProject project, IServiceProxyModel model) : base(TemplateId, project, model)
        {
        }

        public void BeforeTemplateExecution()
        {
            Types.AddClassTypeSource(TypescriptTypeSource.InProject(Project, AngularDTOTemplate.AngularDTOTemplate.TemplateId, "{0}[]"));
        }

        public override string RunTemplate()
        {
            var meta = GetMetaData();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var editor = new AngularModuleEditor(source);
            foreach (var operation in Model.Operations)
            {
                if (editor.MethodExists(operation.Name.ToCamelCase()))
                {
                    continue;
                }
                var url = $"api/{Model.Name.ToLower()}/{operation.Name.ToLower()}";
                editor.AddMethod($@"

  {operation.Name.ToCamelCase()}({GetParameterDefinitions(operation)}): {GetReturnType(operation)} {{
    let url = ""{url}"";
    {GetUpdateUrl(operation)}
    return this.dataService.{GetDataServiceCall(operation)}
      .pipe(map((response: {GetReturnType(operation)}) => {{
        return response;
      }}));
  }}");
            }

            var dependencies = Types.GetTemplateDependencies().Select(x => Project.FindTemplateInstance<ITemplate>(x));
            foreach (var template in dependencies)
            {
                if (!(template is IHasClassDetails))
                {
                    continue;
                }

                editor.AddImportIfNotExists(((IHasClassDetails)template).ClassName, template.GetMetaData().LocationInProject.Replace("\\", "/")); // Temporary replacement until 1.9 changes are merged.
            }


            return editor.GetSource();
        }

        private string GetReturnType(IOperation operation)
        {
            if (operation.ReturnType == null)
            {
                return "boolean";
            }

            return Types.Get(operation.ReturnType.Type);

        }

        private string GetParameterDefinitions(IOperation operation)
        {
            return string.Join(", ", operation.Parameters.Select(x => x.Name.ToCamelCase() + (x.Type.IsNullable ? "?" : "") + ": " + Types.Get(x.Type, "{0}[]")));
        }

        private string GetUpdateUrl(IOperation operation)
        {
            return (GetHttpVerb(operation) == HttpVerb.GET || GetHttpVerb(operation) == HttpVerb.DELETE ? @"url = url + ""?pageNumber="" + pageNumber + ""&pageSize="" + pageSize;" : "");
        }

        private string GetDataServiceCall(IOperation operation)
        {
            switch (GetHttpVerb(operation))
            {
                case HttpVerb.GET:
                    return $"get(url)";
                case HttpVerb.POST:
                    return $"post(url, {operation.Parameters.FirstOrDefault()?.Name.ToCamelCase() ?? "null"})";
                case HttpVerb.PUT:
                    return $"put(url, {operation.Parameters.FirstOrDefault()?.Name.ToCamelCase() ?? "null"})";
                case HttpVerb.DELETE:
                    return $"delete(url)";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name.ToAngularFileName()}.service",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"Client\\src\\app\\{Model.Module.GetModuleName().ToAngularFileName()}",
                className: "${Model.Name}ServiceProxy"
            );
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : TransformText();
        }

        private HttpVerb GetHttpVerb(IOperation operation)
        {
            var verb = operation.GetStereotypeProperty("Http", "Verb", "AUTO").ToUpper();
            if (verb != "AUTO")
            {
                return Enum.TryParse(verb, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }
            if (operation.ReturnType == null || operation.Parameters.Any(x => x.Type.SpecializationType == "DTO"))
            {
                var hasIdParam = operation.Parameters.Any(x => x.Name.ToLower().EndsWith("id") && x.Type.SpecializationType != "DTO");
                if (hasIdParam && new[] { "delete", "remove" }.Any(x => operation.Name.ToLower().Contains(x)))
                {
                    return HttpVerb.DELETE;
                }
                return hasIdParam ? HttpVerb.PUT : HttpVerb.POST;
            }
            return HttpVerb.GET;
        }

        private string GetPath(IOperation operation)
        {
            var path = operation.GetStereotypeProperty<string>("Http", "Route")?.ToLower();
            return path ?? operation.Name.ToLower();
        }
    }

    internal enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}