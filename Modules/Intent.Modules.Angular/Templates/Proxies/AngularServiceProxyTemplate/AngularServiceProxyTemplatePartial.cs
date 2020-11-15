using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Intent.Code.Weaving.TypeScript.Editor;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Angular.Api;
using Intent.Modules.Angular.Templates.Core.ApiServiceTemplate;
using Intent.Modules.Angular.Templates.Module.AngularModuleTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Utils;
using Intent.Modules.Common.TypeScript.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Proxies.AngularServiceProxyTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class AngularServiceProxyTemplate : TypeScriptTemplateBase<ServiceProxyModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Proxies.AngularServiceProxyTemplate.AngularServiceProxyTemplate";

        public AngularServiceProxyTemplate(IOutputTarget project, ServiceProxyModel model) : base(TemplateId, project, model)
        {
            AddTypeSource(AngularDTOTemplate.AngularDTOTemplate.TemplateId);
        }

        public string ApiServiceClassName => GetTypeName(ApiServiceTemplate.TemplateId);

        public override void BeforeTemplateExecution()
        {
            if (File.Exists(GetMetadata().GetFilePath()))
            {
                return;
            }

            // New Proxy:
            ExecutionContext.EventDispatcher.Publish(AngularServiceProxyCreatedEvent.EventId,
                new Dictionary<string, string>()
                {
                    {AngularServiceProxyCreatedEvent.ModuleId, Model.Module.Id },
                    {AngularServiceProxyCreatedEvent.ModelId, Model.Id},
                });
        }

        protected override TypeScriptFile CreateOutputFile()
        {
            if (Model.MappedService == null)
            {
                Logging.Log.Warning($"{ServiceProxyModel.SpecializationType} [{Model.Name}] is not mapped to an underlying Service");
            }

            foreach (var operation in Model.Operations)
            {
                if (!operation.IsMapped || operation.Mapping == null)
                {
                    Logging.Log.Warning($"Operation [{operation.Name}] on {ServiceProxyModel.SpecializationType} [{Model.Name}] is not mapped to an underlying Service Operation");
                }
            }
            return base.CreateOutputFile();
        }

        //      protected override void ApplyFileChanges(TypescriptFile file)
        //      {
        //          if (Model.MappedService == null)
        //          {
        //              Logging.Log.Warning($"{ServiceProxyModel.SpecializationType} [{Model.Name}] is not mapped to an underlying Service");
        //              return;
        //          }
        //          var @class = file.ClassDeclarations().First();
        //          foreach (var operation in Model.Operations)
        //          {
        //              if (!operation.IsMapped || operation.Mapping == null)
        //              {
        //                  Logging.Log.Warning($"Operation [{operation.Name}] on {ServiceProxyModel.SpecializationType} [{Model.Name}] is not mapped to an underlying Service Operation");
        //                  continue;
        //              }
        //              var url = $"/{Model.MappedService.Name.ToLower()}/{operation.Mapping.Element.Name.ToLower()}";
        //              var method = $@"

        //{operation.Name.ToCamelCase()}({GetParameterDefinitions(operation)}): Observable<{GetReturnType(operation)}> {{
        //  let url = ""{url}"";{GetUpdateUrl(operation)}
        //  return this.apiService.{GetDataServiceCall(operation)}
        //    .pipe(map((response: any) => {{
        //      return response;
        //    }}));
        //}}";
        //              if (@class.MethodExists(operation.Name.ToCamelCase()))
        //              {
        //                  @class.ReplaceMethod(operation.Name.ToCamelCase(), method);
        //              }
        //              else
        //              {
        //                  @class.AddMethod(method);
        //              }
        //          }
        //      }

        private string GetReturnType(ServiceProxyOperationModel operation)
        {
            if (operation.ReturnType == null)
            {
                return "boolean";
            }

            return GetTypeName(operation.ReturnType);
        }

        private string GetParameterDefinitions(ServiceProxyOperationModel operation)
        {
            return string.Join(", ", operation.Parameters.Select(x => x.Name.ToCamelCase() + (x.TypeReference.IsNullable ? "?" : "") + ": " + Types.Get(x.TypeReference, "{0}[]")));
        }

        private string GetUpdateUrl(ServiceProxyOperationModel operation)
        {
            var mappedOperation = new OperationModel((IElement)operation.Mapping.Element);
            if (mappedOperation?.Parameters.Count != operation.Parameters.Count)
            {
                throw new Exception($"Different number of properties for mapped operation [{operation.Name}] on {ServiceProxyModel.SpecializationType} [{Model.Name}]");
            }
            if (!mappedOperation.Parameters.Any() || mappedOperation.Parameters.All(x => x.Type.Element.SpecializationType == DTOModel.SpecializationType))
            {
                return "";
            }

            return $@"
        url = `${{url}}?{string.Join("&", mappedOperation.Parameters.Where(x => x.Type.Element.SpecializationType != DTOModel.SpecializationType)
                .Select((x, index) => $"{x.Name.ToCamelCase()}=${{{operation.Parameters[index].Name.ToCamelCase()}}}"))}`;";
        }

        private string GetDataServiceCall(ServiceProxyOperationModel operation)
        {
            switch (GetHttpVerb(operation))
            {
                case HttpVerb.GET:
                    return $"get(url)";
                case HttpVerb.POST:
                    return $"post(url, {operation.Parameters.FirstOrDefault(x => x.TypeReference.Element.SpecializationType == DTOModel.SpecializationType)?.Name.ToCamelCase() ?? "null"})";
                case HttpVerb.PUT:
                    return $"put(url, {operation.Parameters.FirstOrDefault(x => x.TypeReference.Element.SpecializationType == DTOModel.SpecializationType)?.Name.ToCamelCase() ?? "null"})";
                case HttpVerb.DELETE:
                    return $"delete(url)";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name.ToKebabCase()}.service",
                relativeLocation: $"{Model.Module.GetModuleName().ToKebabCase()}",
                className: "${Model.Name}"
            );
        }

        private HttpVerb GetHttpVerb(ServiceProxyOperationModel operation)
        {
            var verb = operation.GetStereotypeProperty("Http", "Verb", "AUTO").ToUpper();
            if (verb != "AUTO")
            {
                return Enum.TryParse(verb, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }
            if (operation.ReturnType == null || operation.Parameters.Any(x => x.TypeReference.Element.SpecializationType == DTOModel.SpecializationType))
            {
                var hasIdParam = operation.Parameters.Any(x => x.Name.ToLower().EndsWith("id") && x.TypeReference.Element.SpecializationType != DTOModel.SpecializationType);
                if (hasIdParam && new[] { "delete", "remove" }.Any(x => operation.Name.ToLower().Contains(x)))
                {
                    return HttpVerb.DELETE;
                }
                return hasIdParam ? HttpVerb.PUT : HttpVerb.POST;
            }
            return HttpVerb.GET;
        }

        private string GetPath(ServiceProxyOperationModel operation)
        {
            var path = operation.GetStereotypeProperty<string>("Http", "Route")?.ToLower();
            return path ?? $"/{Model.MappedService.Name.ToLower()}/{operation.Mapping.Element.Name.ToLower()}";
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