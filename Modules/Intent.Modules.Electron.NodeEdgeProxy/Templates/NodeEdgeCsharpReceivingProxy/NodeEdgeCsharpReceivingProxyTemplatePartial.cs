using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel;
using Intent.MetaModel.Common;
using Intent.MetaModel.DTO;
using Intent.MetaModel.Service;
using Intent.Packages.Application.ServiceCallHandlers.Templates.ServiceCallHandler;
using Intent.Packages.Application.Contracts;
using Intent.Packages.Application.Contracts.Templates.DTO;
using Intent.Packages.Application.Contracts.Templates.ServiceContract;
using Intent.Packages.Unity.Templates.UnityConfig;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using ServiceContractTemplate = Intent.Packages.Application.Contracts.Legacy.ServiceContract.ServiceContractTemplate;

namespace Intent.Packages.Electron.NodeEdgeProxy.Templates.NodeEdgeCsharpReceivingProxy
{
    partial class NodeEdgeCsharpReceivingProxyTemplate : IntentRoslynProjectItemTemplateBase<ServiceModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.Electron.NodeEdgeProxy.CsharpReceivingProxy";

        public NodeEdgeCsharpReceivingProxyTemplate(ServiceModel model, IProject project)
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

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(UnityConfigTemplate.Identifier),
                TemplateDependancy.OnModel(ServiceContractTemplate.Identifier, Model),
                TemplateDependancy.OnTemplate(DTOTemplate.Identifier)
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

        private string GetTypeName(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            if (typeInfo.IsCollection)
            {
                result = "List<" + result + ">";
            }
            return result;
        }

        private string GetOperationReturnType(IOperationModel o)
        {
            if (o.ReturnType == null)
            {
                return "void";
            }
            return GetTypeName(o.ReturnType.TypeReference);
        }

        private string GetOperationCallParameters(IOperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }

            return o.Parameters.Select(x => $"payload.{x.Name}").Aggregate((x, y) => x + ", " + y);
        }


    }
}
