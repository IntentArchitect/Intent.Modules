using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Runtime.InteropServices;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Application.Contracts.Templates.DTO;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.Application.Contracts.Templates.ServiceContract
{
    partial class ServiceContractTemplate : CSharpTemplateBase<ServiceModel>, ITemplate, ITemplatePostCreationHook, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IServiceContractAttributeDecorator>
    {
        private IList<IServiceContractAttributeDecorator> _decorators = new List<IServiceContractAttributeDecorator>();

        public const string IDENTIFIER = "Intent.Application.Contracts.ServiceContract";

        public ServiceContractTemplate(IProject project, ServiceModel model, string identifier = IDENTIFIER)
            : base(identifier, project, model)
        {
            AddTypeSource(CSharpTypeSource.Create(ExecutionContext, DTOTemplate.IDENTIFIER, "List<{0}>"));
            SetDefaultTypeCollectionFormat("List<{0}>");
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new ITemplateDependency[] { };
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new List<INugetPackageInfo>()
            {
                NugetPackages.IntentRoslynWeaverAttributes,
            };
        }

        public IEnumerable<IServiceContractAttributeDecorator> GetDecorators()
        {
            return _decorators;
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"I{Model.Name}",
                @namespace: $"{FolderBasedNamespace}",
                relativeLocation: string.Join("/", GetNamespaceParts()));
        }

        //protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        //{
        //    return new RoslynDefaultFileMetadata(
        //        overwriteBehaviour: OverwriteBehaviour.Always,
        //        fileName: "I${Model.Name}",
        //        fileExtension: "cs",
        //        relativeLocation: string.Join("/", GetNamespaceParts().DefaultIfEmpty("ServiceContracts")),
        //        className: "I${Model.Name}",
        //        @namespace: "${FolderBasedNamespace}");
        //}

        public string FolderBasedNamespace => string.Join(".", new[] { OutputTarget.GetNamespace() }.Concat(GetNamespaceParts()));

        public string ContractAttributes()
        {
            return _decorators.Aggregate(x => x.ContractAttributes(Model));
        }

        public string OperationAttributes(OperationModel operation)
        {
            return _decorators.Aggregate(x => x.OperationAttributes(Model, operation));
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model
                .GetFolderPath(includePackage: false)
                .Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") ?? x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }


        private string GetOperationDefinitionParameters(OperationModel o)
        {
            if (!o.Parameters.Any())
            {
                return "";
            }
            return o.Parameters.Select(x => $"{GetTypeName(x.TypeReference)} {x.Name}").Aggregate((x, y) => x + ", " + y);
        }

        private string GetOperationReturnType(OperationModel o)
        {
            if (o.ReturnType == null)
            {
                return o.IsAsync() ? "Task" : "void";
            }
            return o.IsAsync() ? $"Task<{GetTypeName(o.ReturnType)}>" : GetTypeName(o.TypeReference);
        }

        //private string GetTypeName(ITypeReference typeInfo)
        //{
        //    var result = NormalizeNamespace(Types.Get(typeInfo, "List<{0}>"));
        //    //if (typeInfo.IsCollection && typeInfo.Type != ReferenceType.ClassType)
        //    //{
        //    //    result = string.Format(GetCollectionTypeFormatConfig(), result);
        //    //}
        //    // Don't check for nullables here because the type resolution system will take care of language specific nullables

        //    return result;
        //}

        private string GetCollectionTypeFormatConfig()
        {
            var format = FileMetadata.CustomMetadata["Collection Type Format"];
            if (string.IsNullOrEmpty(format))
            {
                throw new Exception("Collection Type Format not specified in module configuration");
            }
            return format;
        }

        public void AddDecorator(IServiceContractAttributeDecorator decorator)
        {
            _decorators.Add(decorator);
        }
    }
}
