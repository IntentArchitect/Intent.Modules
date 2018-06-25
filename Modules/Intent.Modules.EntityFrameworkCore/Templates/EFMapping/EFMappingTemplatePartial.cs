using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.EntityFramework;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;

namespace Intent.Modules.EntityFrameworkCore.Templates.EFMapping
{
    partial class EFMappingTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IEFMappingTemplateDecorator>, IPostTemplateCreation
    {
        public const string Identifier = "Intent.EntityFrameworkCore.EFMapping";
        private IEnumerable<IEFMappingTemplateDecorator> _decorators;
        private ITemplateDependancy _domainTemplateDependancy;

        public EFMappingTemplate(IClass model, IProject project)
            : base (Identifier, project, model)
        {
            var x = Model.AssociatedClasses.Where(ae => ae.Association.AssociationType == AssociationType.Composition && ae.Association.TargetEnd == ae).ToList();
        }

        public void Created()
        {
            _domainTemplateDependancy = TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == Model.Id);
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

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
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

        private void IssueManyToManyWarning(IAssociationEnd associationEnd)
        {
            Logging.Log.Warning($@"Intent.EntityFrameworkCore: Cannot create mapping relationship from {Model.Name} to {associationEnd.Class.Name}.
  Many-to-Many relationships are not yet supported in EntityFrameworkCore. 
  You will need to create a joining-table entity.
  For more information, please see https://github.com/aspnet/EntityFrameworkCore/issues/1368");
        }
    }

    public interface IEFMappingTemplateDecorator : ITemplateDecorator
    {
        string[] PropertyMappings(Class @class);
    }
}
