using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.RichDomain.Templates.EntityBehaviourBase
{
    partial class DomainEntityBehaviourBaseTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate, IHasDecorators<IDomainEntityBehaviourBaseDecorator>
    {
        private IEnumerable<IDomainEntityBehaviourBaseDecorator> _decorators;
        public const string Identifier = "Intent.RichDomain.EntityBehaviourBase";

        public DomainEntityBehaviourBaseTemplate(Class model, IProject project)
            : base(Identifier, project, model)
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
                fileName: $"{Model.Name}Behaviour",
                fileExtension: "cs",
                defaultLocationInProject: "Generated\\Domain"
                );
        }

        public IEnumerable<IDomainEntityBehaviourBaseDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        public string PublicProperties(Class @class)
        {
            return GetDecorators().Aggregate(x => x.PublicProperties(@class));
        }
    }

    public interface IDomainEntityBehaviourBaseDecorator : ITemplateDecorator
    {
        string[] PublicProperties(Class @class);
    }
}
