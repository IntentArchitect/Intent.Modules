using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Entities.Templates.DomainEntityBehaviour
{
    partial class DomainEntityBehaviourTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate//, IHasDecorators<AbstractDomainEntityDecorator>
    {
        public const string Identifier = "Intent.Entities.Behaviour";
        //private IEnumerable<AbstractDomainEntityDecorator> _decorators;

        public DomainEntityBehaviourTemplate(IClass model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public string ClassStateName => Model.Name;

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, new TemplateVersion(1, 0)));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "${Model.Name}Behaviours",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "${Model.Name}Behaviours",
                @namespace: "${Project.ProjectName}"
                );
        }

        //public IEnumerable<AbstractDomainEntityDecorator> GetDecorators()
        //{
        //    return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        //}

        //public string GetBaseClass(IClass @class)
        //{
        //    return GetDecorators().Select(x => x.GetBaseClass(@class)).FirstOrDefault(x => x != null) ?? @class.ParentClass?.Name ?? "Object";
        //}

        //public string GetInterfaces(IClass @class)
        //{
        //    var interfaces = GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList();
        //    return interfaces.Any() ? ", " + interfaces.Aggregate((x, y) => x + ", " + y) : "";
        //}

        //public string ClassAnnotations(IClass @class)
        //{
        //    return GetDecorators().Aggregate(x => x.ClassAnnotations(@class));
        //}

        //public string PropertyFieldAnnotations(IAttribute attribute)
        //{ 
        //    return GetDecorators().Aggregate(x => x.PropertyFieldAnnotations(attribute));
        //}

        //public string PropertyAnnotations(IAttribute attribute)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        //}

        //public string PropertySetterBefore(IAttribute attribute)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertySetterBefore(attribute));
        //}

        //public string PropertySetterAfter(IAttribute attribute)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertySetterAfter(attribute));
        //}

        //public string PropertyAnnotations(IAssociationEnd associationEnd)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        //}

        //public string PropertySetterBefore(IAssociationEnd associationEnd)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertySetterBefore(associationEnd));
        //}

        //public string PropertySetterAfter(IAssociationEnd associationEnd)
        //{
        //    return GetDecorators().Aggregate(x => x.PropertySetterAfter(associationEnd));
        //}
    }
}
