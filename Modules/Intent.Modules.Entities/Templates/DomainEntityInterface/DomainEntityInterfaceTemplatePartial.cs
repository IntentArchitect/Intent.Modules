using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Domain;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Entities.Templates.DomainEntityInterface
{
    partial class DomainEntityInterfaceTemplate : IntentRoslynProjectItemTemplateBase<IClass>, ITemplate, IHasDecorators<AbstractDomainEntityInterfaceDecorator>
    {

        public const string Identifier = "Intent.Entities.Base.Interface";

        private IEnumerable<AbstractDomainEntityInterfaceDecorator> _decorators;

        public DomainEntityInterfaceTemplate(IClass model, IProject project)
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
                fileName: "I${Name}",
                fileExtension: "cs",
                defaultLocationInProject: "Domain",
                className: "${Name}",
                @namespace: "${Project.ProjectName}"
                );
        }

        public IEnumerable<AbstractDomainEntityInterfaceDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        public string GetInterfaces(IClass @class)
        {
            var interfaces = GetDecorators().SelectMany(x => x.GetInterfaces(@class)).Distinct().ToList();
            return interfaces.Any() ? " : " + interfaces.Aggregate((x, y) => x + ", " + y) : "";
        }

        public string InterfaceAnnotations(IClass @class)
        {
            return GetDecorators().Aggregate(x => x.InterfaceAnnotations(@class));
        }

        public string PropertyAnnotations(IAttribute attribute)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(attribute));
        }

        public string PropertyAnnotations(IAssociationEnd associationEnd)
        {
            return GetDecorators().Aggregate(x => x.PropertyAnnotations(associationEnd));
        }
    }
}
