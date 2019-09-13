using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextStatic
{
    partial class UserContextStaticTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.UserContext.UserContextStatic";

        public UserContextStaticTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                       overwriteBehaviour: OverwriteBehaviour.Always,
                       fileName: $"UserContext",
                       fileExtension: "cs",
                       defaultLocationInProject: "Context"
                );
        }
    }
}
