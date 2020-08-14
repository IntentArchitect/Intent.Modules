using System;
using Intent.Modules.Common.Templates;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Common;
using Intent.Modules.VisualStudio.Projects.Api;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Templates;

namespace Intent.Modules.VisualStudio.Projects.Templates.CoreWeb.CsProject
{
    partial class CoreWebCSProjectTemplate : VisualStudioProjectTemplateBase, ITemplate
    {
        public const string Identifier = "Intent.VisualStudio.Projects.CoreWeb.CSProject";

        public CoreWebCSProjectTemplate(IProject project, ASPNETCoreWebApplicationModel model)
            : base (Identifier, project, model)
        {
        }
    }
}
