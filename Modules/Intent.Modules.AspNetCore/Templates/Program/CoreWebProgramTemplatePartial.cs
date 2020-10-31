using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.Templates.Program
{
    partial class CoreWebProgramTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.AspNetCore.Program";

        public CoreWebProgramTemplate(IProject project)
            : base(Identifier, project, null)
        {
        }

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"Program",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
