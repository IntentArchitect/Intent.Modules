using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates
using System.Collections.Generic;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.RichDomain.Templates.EntityBehaviour
{
    partial class DomainEntityBehaviourTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate, IHasDecorators<IDomainBehaviourDecorator>
    {
        public const string Identifier = "Intent.RichDomain.EntityBehaviour";
        private IEnumerable<IDomainBehaviourDecorator> _decorators;

        public DomainEntityBehaviourTemplate(Class model, IProject project)
            : base (Identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Behaviour",
                className: "N/A", // There are two classes in this template.
                @namespace: "${Project.Name}"
            );
        }

        public string OperationStart()
        {
            return GetDecorators().Aggregate(x => x.OperationStart(Model));
        }

        public IEnumerable<IDomainBehaviourDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }
    }
}
