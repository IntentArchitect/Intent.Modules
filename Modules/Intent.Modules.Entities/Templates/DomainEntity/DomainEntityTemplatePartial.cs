using System.Collections.Generic;
using Intent.MetaModel.Domain;
using Intent.Modules.Entities.Templates.DomainEntityState;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntity
{
    partial class DomainEntityTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasDecorators<DomainEntityDecoratorBase>
    {
        public const string Identifier = "Intent.Entities.DomainEntity";
        private IEnumerable<DomainEntityDecoratorBase> _decorators;

        public DomainEntityTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "${Model.Name}",
                @namespace: "${Project.ProjectName}"
                );
        }

        public IEnumerable<DomainEntityDecoratorBase> GetDecorators()
        {
            if (_decorators == null)
            {
                _decorators = Project.ResolveDecorators(this);
                foreach (var decorator in _decorators)
                {
                    decorator.Template = this;
                }
            }
            return _decorators;
        }

        public string Constructors(IClass @class)
        {
            return GetDecorators().Aggregate(x => x.Constructors(@class));
        }

    }
}
