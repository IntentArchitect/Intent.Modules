using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
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

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"Program",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }
    }
}
