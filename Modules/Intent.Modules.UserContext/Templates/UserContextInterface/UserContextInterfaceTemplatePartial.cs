using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.SoftwareFactory.Templates;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextInterface
{
    partial class UserContextInterfaceTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.UserContext.UserContextInterface";

        public UserContextInterfaceTemplate(IProject project)
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
                       fileName: $"IUserContextData",
                       fileExtension: "cs",
                       defaultLocationInProject: "Context",
                       className: "IUserContextData",
                       @namespace: "${Project.ProjectName}"
                );
        }
    }
}
