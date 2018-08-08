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
using System;

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    partial class ServiceContractTemplate : IntentRoslynProjectItemTemplateBase<IServiceModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IServiceContractAttributeDecorator>
    {
        private readonly string _dtoTemplateId;
        public const string Identifier = "Intent.Application.Contracts.ServiceContract";

        private readonly DecoratorDispatcher<IServiceContractAttributeDecorator> _decoratorDispatcher;

        public ServiceContractTemplate(IProject project, IServiceModel model, string identifier = Identifier, string dtoTemplateId = DTOTemplate.Identifier)
            : base(identifier, project, model)
        {
            _dtoTemplateId = dtoTemplateId;

            _decoratorDispatcher = new DecoratorDispatcher<IServiceContractAttributeDecorator>(project.ResolveDecorators<IServiceContractAttributeDecorator>);
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new ITemplateDependancy[] { };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>()
            {
                NugetPackages.IntentRoslynWeaverAttributes,
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public IEnumerable<IServiceContractAttributeDecorator> GetDecorators()
        {
            return _decoratorDispatcher.GetDecorators();
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "I${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: string.Join("\\", GetNamespaceParts().DefaultIfEmpty("ServiceContracts")),
                className: "I${Model.Name}",
                @namespace: "${FolderBasedNamespace}");
        }

        public string FolderBasedNamespace => string.Join(".", new[] { Project.Name }.Concat(GetNamespaceParts()));

        public string ContractAttributes()
        {
            return _decoratorDispatcher.Dispatch(x => x.ContractAttributes(Model));
        }

        public string OperationAttributes(IOperationModel operation)
        {
            return _decoratorDispatcher.Dispatch(x => x.OperationAttributes(Model, operation));
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model
                .GetFolderPath(includePackage: false)
                .Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") ?? x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));
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
                result = string.Format(GetCollectionTypeFormatConfig(), result);
            }
            else if (typeInfo.IsNullable)
            {
                result = string.Format("System.Nullable<{0}>", result);
            }
            return result;
        }

        private string GetCollectionTypeFormatConfig()
        {
            var format = FileMetaData.CustomMetaData["Collection Type Format"];
            if (string.IsNullOrEmpty(format))
            {
                throw new Exception("Collection Type Format not specified in module configuration");
            }
            return format;
        }
    }
}
