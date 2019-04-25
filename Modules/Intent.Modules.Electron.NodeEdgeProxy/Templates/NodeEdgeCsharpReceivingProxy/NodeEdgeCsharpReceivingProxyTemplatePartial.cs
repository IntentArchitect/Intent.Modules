using Intent.MetaModel.Common;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Application.Contracts.Templates.ServiceContract;
using Intent.Modules.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.Templates
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy
{
    partial class NodeEdgeCsharpReceivingProxyTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Electron.NodeEdgeProxy.CsharpReceivingProxy";

        public NodeEdgeCsharpReceivingProxyTemplate(IServiceModel model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Identifier, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}NodeEdgeProxy",
                fileExtension: "cs",
                defaultLocationInProject: @"NodeEdgeProxies\Generated",
                className: $"{Model.Name}NodeEdgeProxy",
                @namespace: "${Project.ProjectName}");
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(UnityConfigTemplate.Identifier),
                TemplateDependancy.OnModel(ServiceContractTemplate.IDENTIFIER, Model),
                TemplateDependancy.OnTemplate(DTOTemplate.IDENTIFIER)
            };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.NewtonsoftJson,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        private string GetOperationCallParameters(IOperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return string.Empty;
            }

            return o.Parameters
                .Select((x, i) => $"Deserialize<{GetTypeName(x.TypeReference)}>(methodParameters[{i}])")
                .Aggregate((x, y) => x + ", " + y);
        }

        private string GetTypeName(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            if (typeInfo.IsCollection)
            {
                result = "List<" + result + ">";
            }
            return result;
        }
    }
}
