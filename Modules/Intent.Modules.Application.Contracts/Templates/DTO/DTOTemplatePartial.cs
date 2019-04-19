using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.MetaModel.Common;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    partial class DTOTemplate : IntentRoslynProjectItemTemplateBase<IDTOModel>, ITemplate, IHasAssemblyDependencies, IHasDecorators<IDTOAttributeDecorator>
    {
        public const string IDENTIFIER = "Intent.Application.Contracts.DTO";

        private readonly DecoratorDispatcher<IDTOAttributeDecorator> _decoratorDispatcher;

        public DTOTemplate(IProject project, IDTOModel model, string identifier = IDENTIFIER)
            : base(identifier, project, model)
        {
            _decoratorDispatcher = new DecoratorDispatcher<IDTOAttributeDecorator>(project.ResolveDecorators<IDTOAttributeDecorator>);
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new IAssemblyReference[]
            {
                new GacAssemblyReference("System.Runtime.Serialization"),
            };
        }

        public string ClassAttributes()
        {
            return _decoratorDispatcher.Dispatch(x => x.ClasssAttributes(Model));
        }

        public string PropertyAttributes(IAttribute field)
        {
            return _decoratorDispatcher.Dispatch(x => x.PropertyAttributes(Model, field));
        }

        public string ConstructorParameters()
        {
            return Model.Fields.Any()
                ? Model.Fields
                    .Select(x => "\r\n            " + GetTypeInfo(x.Type) + " " + x.Name.ToCamelCase().PrefixIdentifierIfKeyword())
                    .Aggregate((x, y) => x + ", " + y)
                : "";
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: string.Join("\\", GetNamespaceParts().DefaultIfEmpty("DTOs")),
                className: "${Model.Name}",
                @namespace: "${FolderBasedNamespace}");
        }

        public string FolderBasedNamespace => string.Join(".", new[] { Project.Name }.Concat(GetNamespaceParts()));
        public string GenericTypes => Model.GenericTypes.Any() ? $"<{ string.Join(", ", Model.GenericTypes) }>" : "";

        public IEnumerable<IDTOAttributeDecorator> GetDecorators()
        {
            return _decoratorDispatcher.GetDecorators();
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model
                .GetFolderPath(includePackage: false)
                .Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") ?? x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        //private string ResolveNamespace()
        //{
        //    string value;

        //    return GetMetaData().CustomMetaData.TryGetValue("Namespace", out value)
        //        ? value
        //        : FolderBasedNamespace;
        //}

        private string GetTypeInfo(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(typeInfo.GetQualifiedName(this));
            if (typeInfo.IsCollection)
            {
                result = string.Format(GetCollectionTypeFormatConfig(), result);
            }
            // Don't check for nullables here because the type resolution system will take care of language specific nullables

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
