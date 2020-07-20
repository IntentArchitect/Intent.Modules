using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp;

namespace Intent.Modules.Common.VisualStudio
{
    public static class VisualStudioProjectExtensions
    {
        private const string DEPENDENCIES = "VS.Dependencies";
        private const string NUGET_PACKAGES = "VS.NugetPackages";
        private const string REFERENCES = "VS.References";

        public static void InitializeVSMetadata(this IOutputContext project)
        {
            project.Metadata[NUGET_PACKAGES] = new List<INugetPackageInfo>();
            project.Metadata[DEPENDENCIES] = new List<IOutputContext>();
            project.Metadata[REFERENCES] = new List<IAssemblyReference>();
        }

        public static IList<IOutputContext> Dependencies(this IOutputContext project)
        {
            return project.Metadata[DEPENDENCIES] as IList<IOutputContext>;
        }

        public static void AddDependency(this IOutputContext project, IOutputContext dependency)
        {
            var collection = project.Dependencies();
            if (!collection.Contains(dependency))
            {
                collection.Add(dependency);
            }
        }


        public static void AddNugetPackages(this IOutputContext project, IEnumerable<INugetPackageInfo> packages)
        {
            var collection = project.NugetPackages();
            collection.AddRange(packages);
        }

        public static void AddReference(this IOutputContext project, IAssemblyReference assemblyDependency)
        {
            var collection = project.References();
            if (!collection.Contains(assemblyDependency))
                collection.Add(assemblyDependency);
        }

        public static List<INugetPackageInfo> NugetPackages(this IOutputContext project)
        {
            return project.Metadata[NUGET_PACKAGES] as List<INugetPackageInfo>;
        }

        /*
        public static IList<IBowerPackageInfo> BowerPackages(this IProject project)
        {
            return project.TemplateInstances
                    .SelectMany(ti => ti.GetAllBowerDependencies())
                    .Distinct()
                    .ToList();
        }*/

        public static IList<IAssemblyReference> References(this IOutputContext project)
        {
            return project.Metadata[REFERENCES] as IList<IAssemblyReference>;
        }

        //public static string SolutionFolder(this IProject project)
        //{
        //    return project.Folder.Name;
        //}

        //public static string TargetFrameworkVersion(this IProject project)
        //{
        //    var targetFramework = project.ProjectType.Properties.FirstOrDefault(x => x.Name == "TargetFramework");
        //    return project.GetStereotypeProperty("C# .NET", "FrameworkVersion", targetFramework != null ? $"v{targetFramework.Value}" : "v4.5.2");
        //}

        public static string TargetFramework(this IOutputContext project)
        {
            var targetFramework = project.GetSupportedFrameworks().FirstOrDefault();
            return targetFramework ?? "netcoreapp2.1";
        }

        public static bool IsNetCore2App(this IOutputContext project)
        {
            return project.TargetFramework().StartsWith("netcoreapp2");
        }

        public static bool IsNetCore3App(this IOutputContext project)
        {
            return project.TargetFramework().StartsWith("netcoreapp3");
        }
    }
}
