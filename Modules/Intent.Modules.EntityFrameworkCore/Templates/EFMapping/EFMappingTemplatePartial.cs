using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFramework;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;
using Intent.Utils;
using AssociationType = Intent.Modelers.Domain.Api.AssociationType;
using IAssociationEnd = Intent.Modelers.Domain.Api.IAssociationEnd;
using IClass = Intent.Modelers.Domain.Api.IClass;

namespace Intent.Modules.EntityFrameworkCore.Templates.EFMapping
{
    partial class EFMappingTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IEFMappingTemplateDecorator>, IPostTemplateCreation
    {
        public const string Identifier = "Intent.EntityFrameworkCore.EFMapping";
        private IEnumerable<IEFMappingTemplateDecorator> _decorators;
        private ITemplateDependency _domainTemplateDependancy;

        public EFMappingTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
            var x = Model.AssociatedClasses.Where(ae => ae.Association.AssociationType == AssociationType.Composition && ae.Association.TargetEnd == ae).ToList();
        }

        public void Created()
        {
            _domainTemplateDependancy = TemplateDependency.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == Model.Id);
        }

        public string EntityStateName => Project.FindTemplateInstance<IHasClassDetails>(_domainTemplateDependancy)?.ClassName ?? Model.Name;

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFrameworkCore,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                _domainTemplateDependancy,
            };
        }

        public bool UseForeignKeys
        {
            get
            {
                string useForeignKeysString;
                bool useForeignKeys;
                if (GetMetaData().CustomMetaData.TryGetValue("Use Foreign Keys", out useForeignKeysString) && bool.TryParse(useForeignKeysString, out useForeignKeys))
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
                if (GetMetaData().CustomMetaData.TryGetValue("Implicit Surrogate Key", out useForeignKeysString) && bool.TryParse(useForeignKeysString, out useForeignKeys))
                {
                    return useForeignKeys;
                }
                return true;
            }
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Mapping",
                fileExtension: "cs",
                defaultLocationInProject: "EFMapping",
                className: "${Model.Name}Mapping",
                @namespace: "${Project.Name}");
        }

        public IEnumerable<IEFMappingTemplateDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        public string PropertyMappings(Class @class)
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

            var overrideTypeStereotype = attribute.Type.GetStereotype("EFMappingOptions");
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

            var overrideTypeStereotype = attribute.Type.GetStereotype("EFMappingOptions");
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
            var columns = associationEnd.GetStereotypeProperty("Foreign Key", "Column Name", associationEnd.OtherEnd().Name().ToPascalCase() + "Id")
                .Split(',')
                .Select(x => x.Trim())
                .ToList();
            if (columns.Count() == 1)
            {
                return $"x => x.{columns.Single()}";
            }
            return $"x => new {{ {string.Join(", ", columns.Select(x => "x."+ x))}}}";
        }

        private void IssueManyToManyWarning(IAssociationEnd associationEnd)
        {
            Logging.Log.Warning($@"Intent.EntityFrameworkCore: Cannot create mapping relationship from {Model.Name} to {associationEnd.Class.Name}. It has been ignored, and will not be persisted.
    Many-to-Many relationships are not yet supported by EntityFrameworkCore as yet.
    You will need to create a joining-table entity (e.g. [{Model.Name}] 1 --> * [{Model.Name}{associationEnd.Class.Name}] * --> 1 [{associationEnd.Class.Name}])
    For more information, please see https://github.com/aspnet/EntityFrameworkCore/issues/1368");
        }
    }

    public interface IEFMappingTemplateDecorator : ITemplateDecorator
    {
        string[] PropertyMappings(Class @class);
    }
}
