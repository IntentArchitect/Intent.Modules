using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Intent.Configuration;
using Intent.Metadata.Models;

namespace Intent.Modules.VisualStudio.Projects.Api
{
    public interface IVisualStudioProject : IMetadataModel, IHasStereotypes
    {
        string Name { get; }
        string Type { get; }
        string ProjectTypeId { get; }
        string RelativeLocation { get; }

        IOutputTargetConfig ToOutputTargetConfig();
        IEnumerable<string> TargetFrameworkVersion();
        IList<RoleModel> Roles { get; }
        IList<TemplateOutputModel> TemplateOutputs { get; }
        IList<FolderModel> Folders { get; }
        SolutionFolderModel ParentFolder { get; }
        VisualStudioSolutionModel Solution { get; }
    }

    public static class VisualStudioProjectExtensions
    {
        public static IList<IOutputTargetRole> GetRoles(this IVisualStudioProject project)
        {
            return project.Roles.Select(x => new ProjectOutput(x.Name, x.Folder?.Name)).Cast<IOutputTargetRole>()
                .Concat(project.Folders.SelectMany(project.GetRolesInFolder))
                .ToList();
        }

        private static IEnumerable<IOutputTargetRole> GetRolesInFolder(this IVisualStudioProject project, FolderModel folder)
        {
            var roles = folder.Roles.Select(x => new ProjectOutput(x.Name, x.Folder?.Name)).ToList<IOutputTargetRole>();
            return roles;
        }

    }
}