using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Templates;

namespace Intent.Modules.UserContext.Templates.UserContextStatic
{
    partial class UserContextStaticTemplate : CSharpTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.UserContext.UserContextStatic";

        public UserContextStaticTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                       className: $"UserContext",
                       @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
