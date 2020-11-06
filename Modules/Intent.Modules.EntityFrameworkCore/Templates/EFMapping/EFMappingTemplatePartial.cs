using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Templates;
using Intent.Utils;
using AssociationType = Intent.Modelers.Domain.Api.AssociationType;

namespace Intent.Modules.EntityFrameworkCore.Templates.EFMapping
{
    partial class EFMappingTemplate : CSharpTemplateBase<ClassModel>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IEFMappingTemplateDecorator>, ITemplatePostCreationHook
    {
        public const string Identifier = "Intent.EntityFrameworkCore.EFMapping";
        private readonly IList<IEFMappingTemplateDecorator> _decorators = new List<IEFMappingTemplateDecorator>();
        private ITemplateDependency _domainTemplateDependancy;

        public EFMappingTemplate(ClassModel model, IProject project)
            : base(Identifier, project, model)
        {
            if (Model.HasStereotype(Stereotypes.Rdbms.Table.Name))
            {
                AddNugetDependency(NugetPackages.EntityFrameworkCoreSqlServer);
            }

            ValidateAssociations();
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Name}Mapping",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public void ValidateAssociations()
        {
            foreach (var associationEnd in Model.AssociatedClasses)
            {
                var association = associationEnd.Association;

                //if there is more than 1 parent association && there are any which are not 0..1->1 (this is a manual inheritance mapping)
                var multipleCompositions = Model.AssociatedClasses
                    .Where(ae => ae.Association.AssociationType == AssociationType.Composition && ReferenceEquals(ae.Association.TargetEnd.Class, Model))
                    .ToArray();

                if (multipleCompositions.Length > 1)
                {
                    var compositionNames = multipleCompositions
                        .Select(x => x.Class.Name)
                        .Aggregate((x, y) => x + ", " + y);
                    throw new Exception($"Unsupported Mapping - {compositionNames} each have a Compositional relationship with {Model.Name}.");
                }

                if (!association.TargetEnd.IsNavigable)
                {
                    throw new Exception($"Unsupported Source Needs to be Navigable to Target relationship  {association} on {association.TargetEnd.Class.Name} ");
                }

                //Unsupported Associations
                if (association.AssociationType == AssociationType.Aggregation &&
                    association.RelationshipString() == "1->0..1")
                {
                    throw new Exception($"Unsupported Aggregation relationship  {association} - this relationship implies composition");
                }

                if (association.AssociationType == AssociationType.Aggregation &&
                    association.RelationshipString() == "1->1")
                {
                    throw new Exception($"Unsupported Aggregation relationship  {association} - this relationship implies composition");
                }

                if (association.AssociationType == AssociationType.Aggregation &&
                    association.RelationshipString() == "1->*")
                {
                    throw new Exception($"Unsupported Aggregation relationship {association}, this relationship implies Composition");
                }

                if (association.AssociationType == AssociationType.Composition &&
                    association.RelationshipString() == "0..1->0..1")
                {
                    throw new Exception($"Unsupported Composition relationship {association}");
                }

                if (association.AssociationType == AssociationType.Composition &&
                    association.RelationshipString() == "0..1->*")
                {
                    throw new Exception($"Unsupported Composition relationship {association}, this relationship implies aggregation");
                }

                if (association.AssociationType == AssociationType.Composition &&
                    association.RelationshipString().StartsWith("*->"))
                {
                    throw new Exception($"Unsupported Composition relationship {association}, this relationship implies aggregation");
                }

                //Navigability Requirement
                if (association.AssociationType == AssociationType.Composition &&
                    association.RelationshipString() == "0..1->1" && !association.SourceEnd.IsNavigable)
                {
                    throw new Exception(
                        $"Unsupported. IsNavigable from Composition Required for Composition relationship {association}");
                }
            }
        }

        public override void OnCreated()
        {
            _domainTemplateDependancy = TemplateDependency.OnModel<ClassModel>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == Model.Id);
        }

        public string EntityStateName => Project.FindTemplateInstance<IClassProvider>(_domainTemplateDependancy)?.ClassName ?? Model.Name;

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFrameworkCore,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public override IEnumerable<ITemplateDependency> GetTemplateDependencies()
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
                if (GetMetadata().CustomMetadata.TryGetValue("Use Foreign Keys", out var useForeignKeysString) && bool.TryParse(useForeignKeysString, out var useForeignKeys))
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
                if (GetMetadata().CustomMetadata.TryGetValue("Implicit Surrogate Key", out var useForeignKeysString) && bool.TryParse(useForeignKeysString, out var useForeignKeys))
                {
                    return useForeignKeys;
                }

                return true;
            }
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
            var overrideAttributeStereotype = attribute.GetStereotype(Stereotypes.EntityFrameworkCore.EFMappingOptions.Name);
            if (overrideAttributeStereotype != null)
            {
                var columnType = overrideAttributeStereotype.GetProperty<string>(Stereotypes.EntityFrameworkCore.EFMappingOptions.Property.ColumnType);
                if (!string.IsNullOrEmpty(columnType))
                {
                    return true;
                }
            }

            var overrideTypeStereotype = attribute.Type.Element.GetStereotype(Stereotypes.EntityFrameworkCore.EFMappingOptions.Name);
            if (overrideTypeStereotype != null)
            {
                var columnType = overrideTypeStereotype.GetProperty<string>(Stereotypes.EntityFrameworkCore.EFMappingOptions.Property.ColumnType);
                if (!string.IsNullOrEmpty(columnType))
                {
                    return true;
                }
            }

            return false;
        }

        private string GetTypeOverride(AttributeModel attribute)
        {
            var overrideAttributeStereotype = attribute.GetStereotype(Stereotypes.EntityFrameworkCore.EFMappingOptions.Name);
            if (overrideAttributeStereotype != null)
            {
                var columnType = overrideAttributeStereotype.GetProperty<string>(Stereotypes.EntityFrameworkCore.EFMappingOptions.Property.ColumnType);
                if (!string.IsNullOrEmpty(columnType))
                {
                    return columnType;
                }
            }

            var overrideTypeStereotype = attribute.Type.Element.GetStereotype(Stereotypes.EntityFrameworkCore.EFMappingOptions.Name);
            if (overrideTypeStereotype != null)
            {
                var columnType = overrideTypeStereotype.GetProperty<string>(Stereotypes.EntityFrameworkCore.EFMappingOptions.Property.ColumnType);
                if (!string.IsNullOrEmpty(columnType))
                {
                    return columnType;
                }
            }

            return string.Empty;
        }

        private static string GetForeignKeyLambda(AssociationEndModel associationEnd)
        {
            var columns = associationEnd.Class.GetStereotypeProperty(
                    stereotypeName: Stereotypes.Rdbms.ForeignKey.Name,
                    propertyName: Stereotypes.Rdbms.ForeignKey.Property.ColumnName,
                    defaultIfNotFound: associationEnd.OtherEnd().Name().ToPascalCase() + "Id")
                .Split(',')
                .Select(x => x.Trim())
                .ToList();

            if (columns.Count == 1)
            {
                return $"x => x.{columns.Single()}";
            }

            return $"x => new {{ {string.Join(", ", columns.Select(x => "x." + x))}}}";
        }

        private void IssueManyToManyWarning(AssociationEndModel associationEnd)
        {
            Logging.Log.Warning($@"Intent.EntityFrameworkCore: Cannot create mapping relationship from {Model.Name} to {associationEnd.Class.Name}. It has been ignored, and will not be persisted.
    Many-to-Many relationships are not yet supported by EntityFrameworkCore as yet.
    You will need to create a joining-table entity (e.g. [{Model.Name}] 1 --> * [{Model.Name}{associationEnd.Class.Name}] * --> 1 [{associationEnd.Class.Name}])
    For more information, please see https://github.com/aspnet/EntityFrameworkCore/issues/1368");
        }
    }

    public interface IEFMappingTemplateDecorator : ITemplateDecorator
    {
        string[] PropertyMappings(ClassModel @class);
    }
}
