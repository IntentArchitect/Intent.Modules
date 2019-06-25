using System.Collections.Generic;
using System.Linq;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.EntityFramework.Repositories.Templates.EntityCompositionVisitor
{
    partial class EntityCompositionVisitorTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<IClass>>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies, IPostTemplateCreation
    {
        public const string Identifier = "Intent.EntityFramework.Repositories.EntityCompositionVisitor";
        private ITemplateDependency[] _entityStateTemplateDependancies;

        public EntityCompositionVisitorTemplate(IEnumerable<IClass> models, IProject project)
            : base (Identifier, project, models)
        {

        }

        public string BoundedContextName => Project.ApplicationName();


        public void Created()
        {
            _entityStateTemplateDependancies = Model.Select(x => TemplateDependency.OnModel<IClass>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == x.Id)).ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _entityStateTemplateDependancies;
        }

        public string GetClassName(IClass @class)
        {
            return Project.FindTemplateInstance<IHasClassDetails>(TemplateDependency.OnModel<IClass>(GetMetadata().CustomMetadata["Entity Template Id"], (to) => to.Id == @class.Id))?.ClassName ?? $"{@class.Name}";
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "EntityCompositionVisitor",
                fileExtension: "cs",
                defaultLocationInProject: "Visitor",
                className: "EntityCompositionVisitor",
                @namespace: "${Project.ProjectName}"
            );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                new NugetPackageInfo("Intent.Framework.EntityFramework", "1.0.1", null),
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
