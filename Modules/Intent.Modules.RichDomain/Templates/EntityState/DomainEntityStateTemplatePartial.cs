using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.RichDomain.Templates.EntityState
{
    partial class DomainEntityStateTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate, IHasDecorators<IDomainEntityStateDecorator>
    {
        public const string Identifier = "Intent.RichDomain.EntityState";

        private IEnumerable<IDomainEntityStateDecorator> _decorators;

        public DomainEntityStateTemplate(Class model, IProject project)
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
                fileName: $"{Model.Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Generated\\State"
                );
        }

        public IEnumerable<IDomainEntityStateDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        public string ClassAnnotations(Class @class)
        {
            return GetDecorators().Aggregate(x => x.ClassAnnotations(@class));
        }

        public string PropertyFieldAnnotations(UmlAttribute attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyFieldAnnotations(attribute));
        }

        public string PropertyAnnotations(UmlAttribute attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        }

        public string PropertySetterBefore(UmlAttribute attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterBefore(attribute));
        }

        public string PropertySetterAfter(UmlAttribute attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertySetterAfter(attribute));
        }

        public string ConstructorWithOrmLoadingParameter(Class @class)
        {
            return GetDecorators().Aggregate(x => x.ConstructorWithOrmLoadingParameter(@class));
        }

        public string PublicProperties(Class @class)
        {
            return GetDecorators().Aggregate(x => x.PublicProperties(@class));
        }
    }

    public interface IDomainEntityStateDecorator : ITemplateDecorator, IDeclareUsings
    {
        string ClassAnnotations(Class @class);
        string ConstructorWithOrmLoadingParameter(Class @class);
        string PropertyFieldAnnotations(UmlAttribute attribute);
        string PropertyAnnotations(UmlAttribute attribute);
        string PropertySetterBefore(UmlAttribute attribute);
        string PropertySetterAfter(UmlAttribute attribute);
        string[] PublicProperties(Class @class);
    }
}
