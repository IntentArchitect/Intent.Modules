using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Engine;
using Intent.Templates

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
            _entityStateTemplateDependancies = Model.Select(x => TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == x.Id)).ToArray();
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return _entityStateTemplateDependancies;
        }

        public string GetClassName(IClass @class)
        {
            return Project.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == @class.Id))?.ClassName ?? $"{@class.Name}";
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
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
