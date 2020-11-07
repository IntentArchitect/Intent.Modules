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
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Modules.Typescript.ServiceAgent.Contracts.Templates.TypescriptDTO;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    partial class TypescriptWebApiClientServiceProxyTemplate : TypeScriptTemplateBase<ServiceModel>, ITemplate, ITemplateBeforeExecutionHook
    {
        public const string RemoteIdentifier = "Intent.Typescript.ServiceAgent.AngularJs.Proxy.Remote";
        public const string LocalIdentifier = "Intent.Typescript.ServiceAgent.AngularJs.Proxy.Local";

        public const string DomainTemplateDependancyId = "DomainTemplateDependancyId";
        private readonly IApplicationEventDispatcher _eventDispatcher;

        public TypescriptWebApiClientServiceProxyTemplate(string identifier, IProject project, ServiceModel model, IApplicationEventDispatcher eventDispatcher)
            : base(identifier, project, model)
        {
            _eventDispatcher = eventDispatcher;
            AddTypeSource(TypescriptTypeSource.Create(ExecutionContext, TypescriptDtoTemplate.LocalIdentifier));
            AddTypeSource(TypescriptTypeSource.Create(ExecutionContext, TypescriptDtoTemplate.RemoteIdentifier));
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new ITemplateDependency[0]; // disable adding on imports when merged
        }

        public string ApiBasePathConfigKey => $"{OutputTarget.Application.SolutionName.ToLower()}_{Model.Application.Name.ToLower()}_api_basepath".AsClassName().ToLower();

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

        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TypeScriptFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Proxy",
                relativeLocation: $"",
                className: "${Model.Name}Proxy",
                @namespace: "App.Proxies"
                );
        }

        public override void BeforeTemplateExecution()
        {
            // Disabled because of breaking legacy dependencies in GetAddress()
            //_eventDispatcher.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            //{
            //    { "Key", ApiBasePathConfigKey },
            //    { "Value", GetAddress() }
            //});
        }

        private HttpVerb GetHttpVerb(OperationModel operation)
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

        //private string GetAddress()
        //{
        //    var useSsl = false;
        //    bool.TryParse(Project.ProjectType.Properties.FirstOrDefault(x => x.Name == "UseSsl")?.Value, out useSsl);
        //    if (useSsl)
        //    {
        //        return "https://localhost:" + (Project.ProjectType.Properties.FirstOrDefault(x => x.Name == "SslPort")?.Value ?? "???");
        //    }
        //    return "http://localhost:" + (Project.ProjectType.Properties.FirstOrDefault(x => x.Name == "Port")?.Value ?? "???");
        //}

        private string GetMethodDefinitionParameters(OperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {GetTypeName(x.Type)}")
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodCallParametersForGet(OperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return "";
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {x.Name.ToCamelCase()}")
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodCallParametersForPost(OperationModel operation)
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
