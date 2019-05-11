using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;

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
                defaultLocationInProject: "Generated/Domain"
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

        private IList<Class> GetInstantiableBehaviours(Class @class)
        {
            var list = new List<Class>();

            // NB: Order is important, we want most most specialized classes first
            if (@class.SubClasses != null && @class.SubClasses.Any())
            {
                list.AddRange(RecursiveGetSubClasses(@class));
            }
            list.Add(@class);

            return list
                .Where(x => !x.IsAbstract)
                .ToList();
        }

        private IEnumerable<Class> RecursiveGetSubClasses(Class @class)
        {
            var returnList = new List<Class>();
            foreach (var subClass in @class.SubClasses)
            {
                // NB: Order is important, we want most most specialized classes first
                if (subClass.SubClasses != null && subClass.SubClasses.Any())
                {
                    returnList.AddRange(RecursiveGetSubClasses(subClass));
                }

                returnList.Add(subClass);
            }

            return returnList;
        }
    }

    public interface IDomainEntityBehaviourBaseDecorator : ITemplateDecorator
    {
        string[] PublicProperties(Class @class);
    }
}
