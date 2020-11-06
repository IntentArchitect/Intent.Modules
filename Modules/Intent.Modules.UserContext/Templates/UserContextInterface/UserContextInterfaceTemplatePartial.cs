using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextInterface
{
    partial class UserContextInterfaceTemplate : CSharpTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.UserContext.UserContextInterface";

        public UserContextInterfaceTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                       overwriteBehaviour: OverwriteBehaviour.Always,
                       fileName: $"IUserContextData",
                       fileExtension: "cs",
                       relativeLocation: "Context",
                       className: "IUserContextData",
                       @namespace: "${Project.ProjectName}"
                );
        }
    }
}
