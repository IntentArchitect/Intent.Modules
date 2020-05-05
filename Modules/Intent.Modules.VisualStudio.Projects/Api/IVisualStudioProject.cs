using System.CodeDom;
using System.Collections.Generic;
using Intent.Configuration;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public interface IVisualStudioProject : IMetadataModel, IHasStereotypes, IHasFolder
    {
        string Name { get; }
        string Type { get; }
        string ProjectTypeId { get; }
        IList<string> Roles { get; }
        string RelativeLocation { get; }

        IProjectConfig ToProjectConfig();
        string TargetFrameworkVersion();
    }
}