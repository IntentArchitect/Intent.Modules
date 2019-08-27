using System;
using Intent.Modelers.Services.Api;
using Intent.Modules.Constants;
using Intent.Modules.Typescript.ServiceAgent.Contracts;
using Intent.Engine;
using Intent.Eventing;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    partial class TypescriptWebApiClientServiceProxyTemplate : IntentTypescriptProjectItemTemplateBase<IServiceModel>, ITemplate, IBeforeTemplateExecutionHook
    {
        public const string RemoteIdentifier = "Intent.Typescript.ServiceAgent.AngularJs.Proxy.Remote";
        public const string LocalIdentifier = "Intent.Typescript.ServiceAgent.AngularJs.Proxy.Local";

        public const string DomainTemplateDependancyId = "DomainTemplateDependancyId";
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public TypescriptWebApiClientServiceProxyTemplate(string identifier, IProject project, IServiceModel model, IApplicationEventDispatcher eventDispatcher)
            : base(identifier, project, model)
        {
            _eventDispatcher = eventDispatcher;
        }

        public string ApiBasePathConfigKey => $"{Project.Application.SolutionName.ToLower()}_{Model.Application.Name.ToLower()}_api_basepath".AsClassName().ToLower();

        public string AngularModule
        {
            get
            {
                string angularModule;
                if (GetMetadata().CustomMetadata.TryGetValue("AngularModule", out angularModule))
                {
                    return angularModule;
                }
                return "App"; // Default Angular Module
            }
        }

        protected override TypescriptDefaultFileMetadata DefineTypescriptDefaultFileMetadata()
        {
            return new TypescriptDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: $"wwwroot/App/Proxies/Generated",
                className: "${Model.Name}Proxy",
                @namespace: "App.Proxies"
                );
        }

        public void BeforeTemplateExecution()
        {
            _eventDispatcher.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                { "Key", ApiBasePathConfigKey },
                { "Value", GetAddress() }
            });
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
                return HttpVerb.POST;
            }
            return HttpVerb.GET;
        }

        private string GetAddress()
        {
            var useSsl = false;
            bool.TryParse(Project.ProjectType.Properties.FirstOrDefault(x => x.Name == "UseSsl")?.Value, out useSsl);
            if (useSsl)
            {
                return "https://localhost:" + (Project.ProjectType.Properties.FirstOrDefault(x => x.Name == "SslPort")?.Value ?? "???");
            }
            return "http://localhost:" + (Project.ProjectType.Properties.FirstOrDefault(x => x.Name == "Port")?.Value ?? "???");
        }

        private string GetMethodDefinitionParameters(IOperation operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {this.ConvertType(x.Type)}")
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodCallParametersForGet(IOperation operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return "{ }";
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {x.Name.ToCamelCase()}")
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodCallParametersForPost(IOperation operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return "{ }";
            }

            return operation.Parameters
                .Select(x => x.Name.ToCamelCase())
                .Aggregate((x, y) => $"{x}, {y}");
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
