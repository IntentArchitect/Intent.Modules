using System;
using System.IO;
using System.Linq;
using Intent.Modules.NuGet.Installer.ReImplementations;
using Intent.SoftwareFactory;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Exceptions;
using NuGet.ProjectManagement;

namespace Intent.Modules.NuGet.Installer.NugetIntegration
{
    public class CustomMsbuildProjectSystem : MSBuildProjectSystem
    {
        private ICanAddFileStrategy _canAddFileStrategy;
        
        public CustomMsbuildProjectSystem(string projectFullPath, INuGetProjectContext projectContext, bool ignoreMissingImports) : base(null, projectFullPath, projectContext, ignoreMissingImports)
        {
        }

        public void ApplyCanAddFileStrategy(ICanAddFileStrategy canAddFileStrategy)
        {
            _canAddFileStrategy = canAddFileStrategy;
        }

        public override void AddFile(string path, Stream stream)
        {
            // Adding of CanAddStrategy to allow preventing of adding files to the solution

            if (_canAddFileStrategy != null && !_canAddFileStrategy.CanAddFile(path))
            {
                return;
            }

            base.AddFile(path, stream);

            var rootElement = ProjectRootElement.Open(ProjectUniqueName);
            if (rootElement == null)
            {
                throw new Exception("rootElement unexpectedly null");
            }

            if (rootElement.Items.Any(x => x.Include.Equals(path, StringComparison.InvariantCultureIgnoreCase)))
            {
                return;
            }

            rootElement.AddItem(GetProjectItemType(path), path);
        }

        public override void LoadAssemblies(string msbuildDirectory)
        {
            // NOP

            // NuGet normally tries to find the the latest installed version of Visual Studio, get its MSBuild path then use that latest assembly to load up project files.
            
            // For whatever reason, when those DLLs are used (and I confirmed it was trying at the time the latest VS2017 DLLs), then it fails on this import:
            // <Import Project="$(VSToolsPath)\WebApplications\Microsoft.WebApplication.targets" Condition="'$(VSToolsPath)' != ''" />
            // It for some reason was getting the wrong VSTools path.

            // So, for our module we ship it with a MSBuild dlls available through NuGet, and as they are added as normal assembly dependencies, there is no need to do any kind
            // of dynamic assembly or type loading.
        }

        protected override dynamic GetProject(string projectFile)
        {
            // For the reason stated in LoadAssemblies(...) above, we have no need to dynamically load types, additionally,
            // possibility added to ignore missing imports completely, allowing us to run NuGet with no Visual Studio
            // installed at all.

            var globalProjectCollection = ProjectCollection.GlobalProjectCollection;
            var loadedProjects = globalProjectCollection.GetLoadedProjects(projectFile);
            if (loadedProjects.Count > 0)
            {
                return loadedProjects.First();
            }

            var loadSettings = ProjectLoadSettings.Default | ProjectLoadSettings.DoNotEvaluateElementsWithFalseCondition;
            if (_ignoreMissingImports)
            {
                loadSettings |= ProjectLoadSettings.IgnoreMissingImports;
            }

            try
            {
                // I worked out the parameters to use for this constructor overload by looking at:
                // https://github.com/Microsoft/msbuild/blob/dddc68c22f8470ff87148e19abccdb555a1cbae5/src/XMakeBuildEngine/Definition/Project.cs

                var project = new Project(
                    projectFile: projectFile,
                    globalProperties: null,
                    toolsVersion: null,
                    projectCollection: ProjectCollection.GlobalProjectCollection,
                    loadSettings: loadSettings);

                return project;
            }
            catch (InvalidProjectFileException)
            {
                Logging.Log.Warning("The following error may be due to a Visual Studio update. Please see this issue for more information: https://github.com/IntentSoftware/IntentArchitect/issues/130");
                throw;
            }
        }

        private static string GetProjectItemType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName)?.Substring(1); //remove the '.'
            switch (fileExtension)
            {
                case "cs":
                    return "Compile";
                default:
                    return "Content";
            }
        }
    }
}
