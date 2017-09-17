using Microsoft.Build.Construction;
using NuGet.Common;
using NuGet.ProjectManagement;
using System;
using System.IO;
using System.Linq;

namespace Intent.Modules.NuGet.Installer.NugetIntegration
{
    public class CustomMsbuildProjectSystem : MSBuildProjectSystem, IMSBuildNuGetProjectSystem
    {
        private ICanAddFileStrategy _canAddFileStrategy;

        public CustomMsbuildProjectSystem(string msbuildDirectory, string projectFullPath, INuGetProjectContext projectContext) : base(msbuildDirectory, projectFullPath, projectContext)
        {
        }

        public void ApplyCanAddFileStrategy(ICanAddFileStrategy canAddFileStrategy)
        {
            _canAddFileStrategy = canAddFileStrategy;
        }

        void IMSBuildNuGetProjectSystem.AddFile(string path, Stream stream)
        {
            if (_canAddFileStrategy != null && !_canAddFileStrategy.CanAddFile(path))
            {
                return;
            }

            AddFile(path, stream);

            var rootElement = ProjectRootElement.Open(ProjectUniqueName);
            if (rootElement.Items.Any(x => x.Include.Equals(path, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            rootElement.AddItem(GetProjectItemType(path), path);
        }

        private static string GetProjectItemType(string fileName)
        {
            string fileExtension = Path.GetExtension(fileName)
                .Substring(1); //remove the '.'
            switch (fileExtension)
            {
                //case "ts":
                //    return "TypeScriptCompile";
                case "cs":
                    return "Compile";
                default:
                    return "Content";
            }
        }
    }
}
