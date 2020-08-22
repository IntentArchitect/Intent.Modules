using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.VisualStudio.Projects.Api;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public interface IVisualStudioProjectTemplate
    {
        string ProjectId { get; }
        string Name { get; }
        string FilePath { get; }
        string LoadContent();
        IEnumerable<INugetPackageInfo> RequestedNugetPackages();
    }
}