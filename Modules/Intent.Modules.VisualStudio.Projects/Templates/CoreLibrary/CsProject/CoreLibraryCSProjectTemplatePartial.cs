using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreLibrary.CsProject
{
    partial class CoreLibraryCSProjectTemplate : VisualStudioProjectTemplateBase
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreLibrary.CSProject";

        public CoreLibraryCSProjectTemplate(IProject project, IVisualStudioProject model)
            : base(Identifier, project, model)
        {
        }
    }
}
