using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Constants;

namespace Intent.Modules.Common.VisualStudio
{
    public static class VisualStudioProjectExtensions
    {
        private const string DEPENDENCIES = "VS.Dependencies";
        private const string NUGET_PACKAGES = "VS.NugetPackages";
        private const string REFERENCES = "VS.References";

        public static void InitializeVSMetadata(this IOutputTarget outputTarget)
        {
            outputTarget.Metadata[NUGET_PACKAGES] = new List<INugetPackageInfo>();
            outputTarget.Metadata[DEPENDENCIES] = new List<IOutputTarget>();
            outputTarget.Metadata[REFERENCES] = new List<IAssemblyReference>();
        }

        public static IList<IOutputTarget> Dependencies(this IOutputTarget outputTarget)
        {
            return outputTarget.Metadata[DEPENDENCIES] as IList<IOutputTarget>;
        }

        public static void AddDependency(this IOutputTarget outputTarget, IOutputTarget dependency)
        {
            if (outputTarget.Equals(dependency))
            {
                throw new Exception($"OutputTarget [{outputTarget}] cannot add a dependency to itself");
            }
            var collection = outputTarget.Dependencies();
            if (!collection.Contains(dependency))
            {
                collection.Add(dependency);
            }
        }

        public static void AddNugetPackages(this IOutputTarget outputTarget, IEnumerable<INugetPackageInfo> packages)
        {
            var collection = outputTarget.NugetPackages();
            collection.AddRange(packages);
        }

        public static void AddReference(this IOutputTarget outputTarget, IAssemblyReference assemblyDependency)
        {
            var collection = outputTarget.References();
            if (!collection.Contains(assemblyDependency))
                collection.Add(assemblyDependency);
        }

        public static bool IsVSProject(this IOutputTarget outputTarget)
        {
            return new[] {
                    VisualStudioProjectTypeIds.CSharpLibrary,
                    VisualStudioProjectTypeIds.WebApiApplication,
                    VisualStudioProjectTypeIds.WcfApplication,
                    VisualStudioProjectTypeIds.ConsoleAppNetFramework,
                    VisualStudioProjectTypeIds.NodeJsConsoleApplication,
                    VisualStudioProjectTypeIds.CoreWebApp,
                    VisualStudioProjectTypeIds.CoreCSharpLibrary
                }.Contains(outputTarget.Type);
        }

        public static List<INugetPackageInfo> NugetPackages(this IOutputTarget outputTarget)
        {
            return outputTarget.Metadata[NUGET_PACKAGES] as List<INugetPackageInfo>;
        }

        /*
        public static IList<IBowerPackageInfo> BowerPackages(this IProject outputTarget)
        {
            return outputTarget.TemplateInstances
                    .SelectMany(ti => ti.GetAllBowerDependencies())
                    .Distinct()
                    .ToList();
        }*/

        public static IList<IAssemblyReference> References(this IOutputTarget outputTarget)
        {
            return outputTarget.Metadata[REFERENCES] as IList<IAssemblyReference>;
        }

        //public static string SolutionFolder(this IProject outputTarget)
        //{
        //    return outputTarget.Folder.Name;
        //}

        //public static string TargetFrameworkVersion(this IProject outputTarget)
        //{
        //    var targetFramework = outputTarget.ProjectType.Properties.FirstOrDefault(x => x.Name == "TargetFramework");
        //    return outputTarget.GetStereotypeProperty("C# .NET", "FrameworkVersion", targetFramework != null ? $"v{targetFramework.Value}" : "v4.5.2");
        //}

        public static string TargetFramework(this IOutputTarget outputTarget)
        {
            var targetFramework = outputTarget.GetSupportedFrameworks().FirstOrDefault();
            return targetFramework ?? "netcoreapp2.1";
        }

        public static bool IsNetCore2App(this IOutputTarget outputTarget)
        {
            return outputTarget.GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp2"));
        }

        public static bool IsNetCore3App(this IOutputTarget outputTarget)
        {
            return outputTarget.GetSupportedFrameworks().Any(x => x.StartsWith("netcoreapp3"));
        }
    }
}
