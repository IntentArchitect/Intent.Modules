using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.HttpServiceProxy.Templates.InterceptorInterface;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using Intent.Modules.HttpServiceProxy.Templates.HttpClientServiceInterface;

namespace Intent.Modules.HttpServiceProxy.Legacy.Proxy
{
    partial class WebApiClientServiceProxyTemplate : IntentRoslynProjectItemTemplateBase<ServiceModel>, ITemplate, IHasNugetDependencies, IHasAssemblyDependencies, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.HttpServiceProxy.Proxy.Legacy";

        public WebApiClientServiceProxyTemplate(ServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}WebApiClientProxy",
                fileExtension: "cs",
                defaultLocationInProject: @"Generated\ClientProxies",
                className: "${Model.Name}WebApiClientProxy",
                @namespace: "${Project.Name}"
                );
        }

        private string ApplicationName => Model.ApplicationName;

        private static string GetMethodDefinitionParameters(ServiceOperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => $"{x.Type.FullName} {x.Name}")
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private static string GetMethodCallParameters(ServiceOperationModel operation)
        {
            if (operation.Parameters == null || !operation.Parameters.Any())
            {
                return string.Empty;
            }

            return operation.Parameters
                .Select(x => x.Name)
                .Aggregate((x, y) => $"{x}, {y}");
        }

        private static string GetReadAs(ServiceOperationModel operation)
        {
            if (!operation.UsesRawSignature)
            {
                return $"ReadAsAsync<{operation.ReturnType.FullName}>()";
            }

            if (operation.ReturnType.FullName.Equals(typeof(byte[]).Name, StringComparison.InvariantCultureIgnoreCase) ||
                operation.ReturnType.FullName.Equals(typeof(byte[]).FullName, StringComparison.InvariantCultureIgnoreCase))
            {
                return "ReadAsByteArrayAsync()";
            }

            return $"ReadAsAsync<{operation.ReturnType.FullName}>()";
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.MicrosoftAspNetWebApiClient,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.Net.Http"),
                new GacAssemblyReference("System.Configuration"),
            };
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new List<ITemplateDependancy>
            {
                TemplateDependancy.OnTemplate(HttpProxyInterceptorInterfaceTemplate.IDENTIFIER),
            };
        }

        private string GetHttpClientServiceInterfaceName()
        {
            var template = Project.Application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(HttpClientServiceInterfaceTemplate.IDENTIFIER));
            return NormalizeNamespace($"{template.Namespace}.{template.ClassName}");
        }
    }
}
