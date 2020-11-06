using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Templates.EFMapping
{
    partial class EFMappingTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IEFMappingTemplateDecorator>, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.EntityFramework.EFMapping";
        private IList<IEFMappingTemplateDecorator> _decorators = new List<IEFMappingTemplateDecorator>();

        public EFMappingTemplate(ClassModel model, IProject project)
            : base (Identifier, project, model)
        {
            AddNugetDependency(NugetPackages.EntityFramework);
        }

        public string GetEntityName(ClassModel model)
        {
            return GetTypeName(GetMetadata().CustomMetadata["Entity Template Id"], model);
        }

        public bool UseForeignKeys
        {
            get
            {
                string useForeignKeysString;
                bool useForeignKeys;
                if (GetMetadata().CustomMetadata.TryGetValue("Use Foreign Keys", out useForeignKeysString) && bool.TryParse(useForeignKeysString, out useForeignKeys))
                {
                    return useForeignKeys;
                }
                return true;
            }
        }

        public bool ImplicitSurrogateKey
        {
            get
            {
                string useForeignKeysString;
                bool useForeignKeys;
                if (GetMetadata().CustomMetadata.TryGetValue("Implicit Surrogate Key", out useForeignKeysString) && bool.TryParse(useForeignKeysString, out useForeignKeys))
                {
                    return useForeignKeys;
                }
                return true;
            }
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Mapping",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public void AddDecorator(IEFMappingTemplateDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<IEFMappingTemplateDecorator> GetDecorators()
        {
            return _decorators;
        }

        public string PropertyMappings(ClassModel @class)
        {
            return GetDecorators().Aggregate(x => x.PropertyMappings(@class));
        }

        private bool HasTypeOverride(AttributeModel attribute)
        {
            var overrideAttributeStereotype = attribute.GetStereotype("EFMappingOptions");
            if (overrideAttributeStereotype != null)
            {
                var columnType = overrideAttributeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return true;
                }
            }

            var overrideTypeStereotype = attribute.Type.Element.GetStereotype("EFMappingOptions");
            if (overrideTypeStereotype != null)
            {
                var columnType = overrideTypeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetTypeOverride(AttributeModel attribute)
        {
            var overrideAttributeStereotype = attribute.GetStereotype("EFMappingOptions");
            if (overrideAttributeStereotype != null)
            {
                var columnType = overrideAttributeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return columnType;
                }
            }

            var overrideTypeStereotype = attribute.Type.Element.GetStereotype("EFMappingOptions");
            if (overrideTypeStereotype != null)
            {
                var columnType = overrideTypeStereotype.GetProperty<string>("ColumnType");
                if (!string.IsNullOrEmpty(columnType))
                {
                    return columnType;
                }
            }

            return string.Empty;
        }

        private string GetForeignKeyLambda(AssociationEndModel associationEnd)
        {
            var columns = associationEnd.Element.GetStereotypeProperty("Foreign Key", "Column Name", associationEnd.OtherEnd().Name().ToPascalCase() + "Id")
                .Split(',')
                .Select(x => x.Trim())
                .ToList();
            if (columns.Count() == 1)
            {
                return $"x => x.{columns.Single()}";
            }
            return $"x => new {{ {string.Join(", ", columns.Select(x => "x." + x))}}}";
        }

        private static bool RequiresForeignKeyOnAssociatedEnd(AssociationEndModel associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.Many
                &&
                (associationEnd.Association.AssociationType == AssociationType.Composition || associationEnd.OtherEnd().IsNavigable);
        }
    }

    public interface IEFMappingTemplateDecorator : ITemplateDecorator
    {
        string[] PropertyMappings(ClassModel @class);
    }
}
