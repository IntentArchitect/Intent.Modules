using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    partial class DTOTemplate : IntentRoslynProjectItemTemplateBase<DTOModel>, ITemplate, IHasAssemblyDependencies, IHasDecorators<IDTOAttributeDecorator>, ITemplatePostCreationHook
    {
        public const string IDENTIFIER = "Intent.Application.Contracts.DTO";

        private IList<IDTOAttributeDecorator> _decorators = new List<IDTOAttributeDecorator>();

        public DTOTemplate(IProject project, DTOModel model, string identifier = IDENTIFIER)
            : base(identifier, project, model)
        {
        }

        public override void OnCreated()
        {
            Types.AddClassTypeSource(CSharpTypeSource.Create(ExecutionContext, DTOTemplate.IDENTIFIER, "List<{0}>"));
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
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
            return _decorators.Aggregate(x => x.ClassAttributes(Model));
        }

        public string PropertyAttributes(DTOFieldModel field)
        {
            return _decorators.Aggregate(x => x.PropertyAttributes(Model, field));
        }

        public string ConstructorParameters()
        {
            return Model.Fields.Any()
                ? Model.Fields
                    .Select(x => "\r\n            " + GetTypeInfo(x.TypeReference) + " " + x.Name.ToCamelCase(reservedWordEscape: true))
                    .Aggregate((x, y) => x + ", " + y)
                : "";
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: string.Join("/", GetNamespaceParts().DefaultIfEmpty("DTOs")),
                className: "${Model.Name}",
                @namespace: "${FolderBasedNamespace}");
        }

        public string FolderBasedNamespace => string.Join(".", new[] { Project.Name }.Concat(GetNamespaceParts()));
        public string GenericTypes => Model.GenericTypes.Any() ? $"<{ string.Join(", ", Model.GenericTypes) }>" : "";

        public IEnumerable<IDTOAttributeDecorator> GetDecorators()
        {
            return _decorators;
        }

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model
                .GetFolderPath()
                .Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") ?? x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        //private string ResolveNamespace()
        //{
        //    string value;

        //    return GetMetadata().CustomMetadata.TryGetValue("Namespace", out value)
        //        ? value
        //        : FolderBasedNamespace;
        //}

        private string GetTypeInfo(ITypeReference typeInfo)
        {
            var result = NormalizeNamespace(Types.Get(typeInfo, "List<{0}>").Name);
            // Don't check for nullables here because the type resolution system will take care of language specific nullables

            return result;
        }

        private string GetCollectionTypeFormatConfig()
        {
            var format = FileMetadata.CustomMetadata["Collection Type Format"];
            if (string.IsNullOrEmpty(format))
            {
                throw new Exception("Collection Type Format not specified in module configuration");
            }
            return format;
        }

        public void AddDecorator(IDTOAttributeDecorator decorator)
        {
            _decorators.Add(decorator);
        }
    }
}
