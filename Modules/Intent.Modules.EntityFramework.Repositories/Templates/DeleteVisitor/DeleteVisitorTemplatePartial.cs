using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.VisualStudio;
using Class = Intent.SoftwareFactory.MetaModels.UMLModel.Class;

namespace Intent.Modules.EntityFramework.Repositories.Templates.DeleteVisitor
{
    partial class DeleteVisitorTemplate : IntentRoslynProjectItemTemplateBase<IEnumerable<IClass>>, ITemplate, IHasTemplateDependencies, IHasNugetDependencies
    {
        public const string Identifier = "Intent.RichDomain.EntityFramework.DeleteVisitor";
        private ITemplateDependancy[] _entityStateTemplateDependancies;

        public DeleteVisitorTemplate(IEnumerable<IClass> models, IProject project)
            : base (Identifier, project, models)
        {

        }

        public string BoundedContextName => Project.ApplicationName();


        public void Created()
        {
            _entityStateTemplateDependancies = Model.Select(x => TemplateDependancy.OnModel<IClass>(GetMetaData().CustomMetaData["Entity Template Id"], (to) => to.Id == x.Id)).ToArray();
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
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
                fileName: "${Project.Application.ApplicationName}DeleteVisitor",
                fileExtension: "cs",
                defaultLocationInProject: "DeleteVisitor",
                className: "${Project.Application.ApplicationName}DeleteVisitor",
                @namespace: "${Project.ProjectName}.DeleteVisitor"
            );
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkEntityFramework,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }
    }
}
