using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Angular.Templates.Core.ApiServiceTemplate;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class AngularServiceProxyTemplate : AngularTypescriptProjectItemTemplateBase<IServiceProxyModel>, ITemplateBeforeExecutionHook
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Proxies.AngularServiceProxyTemplate";

        public AngularServiceProxyTemplate(IProject project, IServiceProxyModel model) : base(TemplateId, project, model, TypescriptTemplateMode.UpdateFile)
        {
            AddTypeSource(TypescriptTypeSource.InProject(Project, AngularDTOTemplate.AngularDTOTemplate.TemplateId));
        }

        public string ApiServiceClassName => GetTemplateClassName(ApiServiceTemplate.TemplateId);

        public override void BeforeTemplateExecution()
        {
            if (File.Exists(GetMetadata().GetFullLocationPathWithFileName()))
            {
                return;
            }

            // New Proxy:
            Project.Application.EventDispatcher.Publish(AngularServiceProxyCreatedEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularServiceProxyCreatedEvent.ModuleId, Model.Module.Id },
                    {AngularServiceProxyCreatedEvent.ModelId, Model.Id},
                });
        }

        protected override void ApplyFileChanges(TypescriptFile file)
        {
            if (Model.MappedService == null)
            {
                Logging.Log.Warning($"{ServiceProxyModel.SpecializationType} [{Model.Name}] is not mapped to an underlying Service");
                return;
            }
            var @class = file.ClassDeclarations().First();
            foreach (var operation in Model.Operations)
            {
                if (!operation.IsMapped || operation.Mapping == null)
                {
                    Logging.Log.Warning($"Operation [{operation.Name}] on {ServiceProxyModel.SpecializationType} [{Model.Name}] is not mapped to an underlying Service Operation");
                    continue;
                }
                var url = $"/{Model.MappedService.Name.ToLower()}/{Model.MappedService.Operations.First(x => x.Id == operation.Mapping.TargetId).Name.ToLower()}";
                var method = $@"

  {operation.Name.ToCamelCase()}({GetParameterDefinitions(operation)}): Observable<{GetReturnType(operation)}> {{
    let url = ""{url}"";{GetUpdateUrl(operation)}
    return this.apiService.{GetDataServiceCall(operation)}
      .pipe(map((response: any) => {{
        return response;
      }}));
  }}";
                if (@class.MethodExists(operation.Name.ToCamelCase()))
                {
                    @class.ReplaceMethod(operation.Name.ToCamelCase(), method);
                }
                else
                {
                    @class.AddMethod(method);
                }
            }
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
            var mappedOperation = Model.MappedService?.Operations.FirstOrDefault(x => x.Id == operation.Mapping.TargetId);
            if (mappedOperation?.Parameters.Count != operation.Parameters.Count)
            {
                throw new Exception($"Different number of properties for mapped operation [{operation.Name}] on {ServiceProxyModel.SpecializationType} [{Model.Name}]");
            }
            if (!mappedOperation.Parameters.Any() || mappedOperation.Parameters.All(x => x.Type.Element.IsDTO()))
            {
                return "";
            }

            return $@"
        url = `${{url}}?{string.Join("&", mappedOperation.Parameters.Where(x => !x.Type.Element.IsDTO())
                .Select((x, index) => $"{x.Name.ToCamelCase()}=${{{operation.Parameters[index].Name.ToCamelCase()}}}"))}`;";
        }

        private string GetDataServiceCall(IOperation operation)
        {
            switch (GetHttpVerb(operation))
            {
                case HttpVerb.GET:
                    return $"get(url)";
                case HttpVerb.POST:
                    return $"post(url, {operation.Parameters.FirstOrDefault(x => x.Type.Element.IsDTO())?.Name.ToCamelCase() ?? "null"})";
                case HttpVerb.PUT:
                    return $"put(url, {operation.Parameters.FirstOrDefault(x => x.Type.Element.IsDTO())?.Name.ToCamelCase() ?? "null"})";
                case HttpVerb.DELETE:
                    return $"delete(url)";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: $"{Model.Name.ToKebabCase()}.service",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: $"Client/src/app/{Model.Module.GetModuleName().ToKebabCase()}",
                className: "${Model.Name}"
            );
        }

        private HttpVerb GetHttpVerb(IOperation operation)
        {
            var verb = operation.GetStereotypeProperty("Http", "Verb", "AUTO").ToUpper();
            if (verb != "AUTO")
            {
                return Enum.TryParse(verb, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }
            if (operation.ReturnType == null || operation.Parameters.Any(x => x.Type.Element.SpecializationType == "DTO"))
            {
                var hasIdParam = operation.Parameters.Any(x => x.Name.ToLower().EndsWith("id") && x.Type.Element.SpecializationType != "DTO");
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