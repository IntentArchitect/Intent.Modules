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
        string RelativeLocation { get; }

        IList<string> GetRoles();
        IProjectConfig ToProjectConfig();
        string TargetFrameworkVersion();
    }
}