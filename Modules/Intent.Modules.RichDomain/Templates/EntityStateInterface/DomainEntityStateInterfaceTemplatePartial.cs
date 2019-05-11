#warning Indexes and Unqiues on FKs think its not done for 1 to Many
#warning need to factor in Aggregate boundaries into Edit and States

using Intent.Engine;
using Intent.SoftwareFactory.MetaModels.UMLModel;
using Intent.Templates;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.RichDomain.Templates.EntityStateInterface
{
    partial class DomainEntityStateInterfaceTemplate : IntentRoslynProjectItemTemplateBase<Class>, ITemplate, IHasNugetDependencies, IHasDecorators<IDomainEntityStateInterfaceTemplateDecorator>
    {
        public const string Identifier = "Intent.RichDomain.EntityStateInterface";
        private IEnumerable<IDomainEntityStateInterfaceTemplateDecorator> _decorators;

        public DomainEntityStateInterfaceTemplate(Class model, IProject project)
            : base(Identifier, project, model)
        {
        }

        public override IEnumerable<INugetPackageInfo> GetNugetDependencies()
        {
            return new[]
            {
                NugetPackages.IntentFrameworkDomain,
            }
            .Union(base.GetNugetDependencies())
            .ToArray();
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: $"{Model.Name}Interfaces",
                fileExtension: "cs",
                defaultLocationInProject: "Generated/StateInterfaces",
                className: "I${Model.Name}",
                @namespace: "${Project.ProjectName}"
            );
        }

        public IEnumerable<IDomainEntityStateInterfaceTemplateDecorator> GetDecorators()
        {
            return _decorators ?? (_decorators = Project.ResolveDecorators(this));
        }

        public string InterfaceProperties(Class @class)
        {
            return GetDecorators().Aggregate(x => x.InterfaceProperties(@class));
        }

        public string ImplementationPartialProperties(Class @class, string readOnlyInterfaceName)
        {
            return GetDecorators().Aggregate(x => x.ImplementationPartialProperties(@class, readOnlyInterfaceName));
        }
    }

    public interface IDomainEntityStateInterfaceTemplateDecorator : ITemplateDecorator
    {
        string[] InterfaceProperties(Class @class);
        string[] ImplementationPartialProperties(Class @class, string readOnlyInterfaceName);
    }
}
