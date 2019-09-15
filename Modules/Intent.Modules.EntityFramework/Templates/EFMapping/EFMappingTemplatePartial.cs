using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;
using AssociationType = Intent.Modelers.Domain.Api.AssociationType;
using IAssociationEnd = Intent.Modelers.Domain.Api.IAssociationEnd;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.EntityFramework.Templates.EFMapping
{
    partial class EFMappingTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IEFMappingTemplateDecorator>, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.EntityFramework.EFMapping";
        private IList<IEFMappingTemplateDecorator> _decorators = new List<IEFMappingTemplateDecorator>();
        private ITemplateDependency _domainTemplateDependency;

        public EFMappingTemplate(IClass model, IProject project)
            : base (Identifier, project, model)
        {
            var x = Model.AssociatedClasses.Where(ae => ae.Association.AssociationType == AssociationType.Composition && ae.Association.TargetEnd == ae).ToList();
        }

        public override void OnCreated()
        {
            _domainTemplateDependency = TemplateDependency.OnModel<IClass>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == Model.Id);
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFramework,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                _domainTemplateDependency,
            };
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

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}Mapping",
                fileExtension: "cs",
                defaultLocationInProject: "EFMapping",
                className: $"{Model.Name}Mapping",
                @namespace: "${Project.Name}");
        }

        public void AddDecorator(IEFMappingTemplateDecorator decorator)
        {
            _decorators.Add(decorator);
        }

        public IEnumerable<IEFMappingTemplateDecorator> GetDecorators()
        {
            return _decorators;
        }

        public string PropertyMappings(IClass @class)
        {
            return GetDecorators().Aggregate(x => x.PropertyMappings(@class));
        }

        private bool HasTypeOverride(IAttribute attribute)
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

        private string GetTypeOverride(IAttribute attribute)
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

        private string GetForeignKeyLambda(IAssociationEnd associationEnd)
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

        private static bool RequiresForeignKeyOnAssociatedEnd(IAssociationEnd associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.Many
                &&
                (associationEnd.Association.AssociationType == AssociationType.Composition || associationEnd.OtherEnd().IsNavigable);
        }
    }

    public interface IEFMappingTemplateDecorator : ITemplateDecorator
    {
        string[] PropertyMappings(IClass @class);
    }
}
