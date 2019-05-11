using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.RichDomain.Templates.EntityState;
using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates

namespace Intent.Modules.RichDomain.EntityFramework.Templates.EFMapping
{
    partial class EFMappingTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IHasDecorators<IEFMappingTemplateDecorator>
    {
        public const string Identifier = "Intent.RichDomain.EntityFramework.EFMapping";
        private IEnumerable<IEFMappingTemplateDecorator> _decorators;

        public EFMappingTemplate(Class model, IProject project)
            : base (Identifier, project, model)
        {
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.EntityFramework,
                NugetPackages.IntentFrameworkDomain,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnModel(DomainEntityStateTemplate.Identifier, Model),
            };
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}Mapping",
                fileExtension: "cs",
                defaultLocationInProject: "Generated/EFMapping",
                className: $"{Model.Name}Mapping",
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
    }

    public interface IEFMappingTemplateDecorator : ITemplateDecorator
    {
        string[] PropertyMappings(Class @class);
    }
}
