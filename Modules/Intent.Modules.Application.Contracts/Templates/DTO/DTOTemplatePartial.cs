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
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.Application.Contracts.Templates.DTO
{
    partial class DTOTemplate : CSharpTemplateBase<DTOModel>, ITemplate, IHasAssemblyDependencies, IHasDecorators<IDTOAttributeDecorator>, ITemplatePostCreationHook
    {
        public const string IDENTIFIER = "Intent.Application.Contracts.DTO";

        private IList<IDTOAttributeDecorator> _decorators = new List<IDTOAttributeDecorator>();

        public DTOTemplate(IProject project, DTOModel model, string identifier = IDENTIFIER)
            : base(identifier, project, model)
        {
            AddAssemblyReference(new GacAssemblyReference("System.Runtime.Serialization"));
            AddTypeSource(DTOTemplate.IDENTIFIER, "List<{0}>");
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

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}",
                @namespace: $"{FolderBasedNamespace}",
                relativeLocation: string.Join("/", GetNamespaceParts()));
        }

        //protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        //{
        //    return new RoslynDefaultFileMetadata(
        //        overwriteBehaviour: OverwriteBehaviour.Always,
        //        fileName: "${Model.Name}",
        //        fileExtension: "cs",
        //        relativeLocation: string.Join("/", GetNamespaceParts().DefaultIfEmpty("DTOs")),
        //        className: "${Model.Name}",
        //        @namespace: "${FolderBasedNamespace}");
        //}

        public string FolderBasedNamespace => string.Join(".", new[] { OutputTarget.GetNamespace() }.Concat(GetNamespaceParts()));
        public string GenericTypes => Model.GenericTypes.Any() ? $"<{ string.Join(", ", Model.GenericTypes) }>" : "";

        private IEnumerable<string> GetNamespaceParts()
        {
            return Model
                .GetFolderPath()
                .Select(x => x.GetStereotypeProperty<string>(StandardStereotypes.NamespaceProvider, "Namespace") ?? x.Name)
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        public IEnumerable<IDTOAttributeDecorator> GetDecorators()
        {
            return _decorators;
        }
        
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
