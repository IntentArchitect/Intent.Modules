using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.ConsoleApp.Program
{
    partial class ConsoleAppProgramTemplate : CSharpTemplateBase<object>
    {
        public const string Identifier = "Intent.VisualStudio.Projects.ConsoleApp.Program";

        public ConsoleAppProgramTemplate(IProject project)
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
