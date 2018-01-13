using Intent.MetaModel.Common;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaData;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    partial class ServiceContractTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies
    {
        private readonly string _dtoTemplateId;
        public const string Identifier = "Intent.Application.Contracts.ServiceContract";

        public ServiceContractTemplate(IProject project, IServiceModel model, string identifier = Identifier, string dtoTemplateId = DTOTemplate.Identifier)
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
                fileName: "I${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: string.Join("\\", GetNamespaceParts().DefaultIfEmpty("ServiceContracts")),
                className: "I${Model.Name}",
                @namespace: string.Join(".", new[] { "${Project.ProjectName}" }.Concat(GetNamespaceParts()))
                );
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model.GetFolderPath(includePackage: true).Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace")).Where(x => x != null);
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
