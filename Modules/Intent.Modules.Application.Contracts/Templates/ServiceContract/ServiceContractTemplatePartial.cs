using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.MetaModel.Service;
using Intent.Packages.Application.Contracts.Templates.DTO;
using Intent.Packages.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Packages.Application.Contracts.Templates.ServiceContract
{
    partial class ServiceContractTemplate : IntentRoslynProjectItemTemplateBase<ServiceModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies
    {
        private readonly string _dtoTemplateId;
        public const string Identifier = "Intent.Application.Contracts.ServiceContract";

        public ServiceContractTemplate(IProject project, ServiceModel model, string identifier = Identifier, string dtoTemplateId = DTOTemplate.Identifier)
            : base(identifier, project, model)
        {
            _dtoTemplateId = dtoTemplateId;
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(_dtoTemplateId),
            };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>()
            {
                NugetPackages.IntentFrameworkCore,
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "I${Name}",
                fileExtension: "cs",
                defaultLocationInProject: string.Join("\\", GetNamespaceParts().DefaultIfEmpty("ServiceContracts")),
                className: "I${Name}",
                @namespace: string.Join(".", new[] { "${Project.ProjectName}" }.Concat(GetNamespaceParts()))
                );
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model.GetFolderPath().Select(x => x.GetPropertyValue<string>(StandardStereotypes.NamespaceProvider, "Namespace")).Where(x => x != null);
        }

        private string GetOperationDefinitionParameters(IOperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(IOperationModel o)
        {
            if (o.ReturnType == null)
            {
                return "void";
            }
            return GetTypeName(o.ReturnType.TypeReference);
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
