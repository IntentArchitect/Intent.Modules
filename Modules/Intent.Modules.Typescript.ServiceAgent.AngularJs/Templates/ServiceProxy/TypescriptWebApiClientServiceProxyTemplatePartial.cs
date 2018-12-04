using System;
using Intent.MetaModel.Service;
using Intent.Modules.Constants;
using Intent.Modules.Typescript.ServiceAgent.Contracts;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    partial class TypescriptWebApiClientServiceProxyTemplate : IntentTypescriptProjectItemTemplateBase<IServiceModel>, ITemplate, IRequiresPreProcessing
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
                if (GetMetaData().CustomMetaData.TryGetValue("AngularModule", out angularModule))
                {
                    return angularModule;
                }
                return "App"; // Default Angular Module
            }
        }

        protected override TypescriptDefaultFileMetaData DefineTypescriptDefaultFileMetaData()
        {
            return new TypescriptDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "${Model.Name}Proxy",
                fileExtension: "ts",
                defaultLocationInProject: $@"wwwroot\App\Proxies\Generated",
                className: "${Model.Name}Proxy",
                @namespace: "App.Proxies"
                );
        }

        public void PreProcess()
        {
            _eventDispatcher.Publish(ApplicationEvents.AngularJs_ConfigurationRequired, new Dictionary<string, string>()
            {
                { "Key", ApiBasePathConfigKey },
                { "Value", GetAddress() }
            });
        }

        private HttpVerb GetHttpVerb(IOperationModel operation)
        {
            var verb = operation.GetStereotypeProperty("Http", "Verb", "AUTO").ToUpper();
            if (verb != "AUTO")
            {
                return Enum.TryParse(verb, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }
            if (operation.ReturnType == null || operation.Parameters.Any(x => x.TypeReference.Type == ReferenceType.ClassType))
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

        private string GetMethodDefinitionParameters(IOperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => $"{x.Name.ToCamelCase()}: {this.ConvertType(x.TypeReference)}")
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private string GetMethodCallParameters(IOperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
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
