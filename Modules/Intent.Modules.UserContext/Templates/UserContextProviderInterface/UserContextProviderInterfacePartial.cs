using System.Collections.Generic;
using Intent.Modules.Common.Templates;
using Intent.Modules.Constants;
using Intent.Modules.UserContext.Templates.UserContextInterface;
using Intent.Engine;
using Intent.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextProviderInterface
{
    partial class UserContextProviderInterfaceTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate, IHasTemplateDependencies
    {
        public const string Identifier = "Intent.UserContext.UserContextProviderInterface";


        public UserContextProviderInterfaceTemplate(IProject project)
            : base (Identifier, project, null)
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
                       fileName: $"IUserContextProvider",
                       fileExtension: "cs",
                       defaultLocationInProject: "Providers",
                       className: "IUserContextProvider",
                       @namespace: "${Project.ProjectName}"
                );
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new List<ITemplateDependency>
            {
                TemplateDependency.OnTemplate(UserContextInterfaceTemplate.Identifier),
            };
        }
    }
}
