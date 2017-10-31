using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.VisualStudio
{
    public static class VisualStudioProjectExtensions
    {
        private const string DEPENDENCIES = "VS.Dependencies";
        private const string NUGET_PACKAGES = "VS.NugetPackages";
        private const string REFERENCES = "VS.References";

        public static void InitializeVSMetaData(this IProject project)
        {
            project.MetaData[NUGET_PACKAGES] = new List<INugetPackageInfo>();
            project.MetaData[DEPENDENCIES] = new List<IProject>();
            project.MetaData[REFERENCES] = new List<IAssemblyReference>();
        }

        public static IList<IProject> Dependencies(this IProject project)
        {
            return project.MetaData[DEPENDENCIES] as IList<IProject>;
        }

        public static void AddDependency(this IProject project, IProject dependency)
        {
            var collection = project.Dependencies();
            if (!collection.Contains(dependency))
            {
                    collection.Add(dependency);
            }
        }


        public static void AddNugetPackages(this IProject project, IEnumerable<INugetPackageInfo> packages)
        {
            var collection = project.NugetPackages();
            collection.AddRange(packages);
        }

        public static void AddReference(this IProject project, IAssemblyReference assemblyDependency)
        {
            var collection = project.References();
            if (!collection.Contains(assemblyDependency))
                collection.Add(assemblyDependency);
        }

        public static List<INugetPackageInfo> NugetPackages(this IProject project)
        {
            return project.MetaData[NUGET_PACKAGES] as List<INugetPackageInfo>;
        }

        /*
        public static IList<IBowerPackageInfo> BowerPackages(this IProject project)
        {
            return project.TemplateInstances
                    .SelectMany(ti => ti.GetAllBowerDependencies())
                    .Distinct()
                    .ToList();
        }*/

        public static IList<IAssemblyReference> References(this IProject project)
        {
            return project.MetaData[REFERENCES] as IList<IAssemblyReference>;
        }


    }
}
