using Intent.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Intent.Modules.ModuleBuilder.CSharp.Tasks.NuGetApi;

namespace Intent.Modules.ModuleBuilder.CSharp.Tasks
{
    public class TaskResponse
    {
        public TaskResponse()
        {
            NugetPackages = new List<NugutPackageInfo>();
        }
        public List<NugutPackageInfo> NugetPackages { get; set; }
    }

    public class NugutPackageInfo
    {
        public NugutPackageInfo()
        {
            Versions = new List<NugutPackageVersionInfo>();
        }

        internal NugutPackageInfo(string name, List<NugetVersionInfo> versions)
        {
            Name = name;
            Versions = new List<NugutPackageVersionInfo>();
            foreach (var version in versions)
            {
                Versions.Add(new NugutPackageVersionInfo() 
                { 
                    Version = version.PackageVersion.ToString(), 
                    TargetFramework = version.FrameworkVersion.DotNetFrameworkName, 
                    Dependencies = version.Dependencies.Select(d => new NugutPackageDependendcyInfo() { PackageName = d.PackageName, Version = d.Version.MinVersion.ToString() }).ToList()
                });
            }
        }


        public string Name { get; set; }
        public List<NugutPackageVersionInfo> Versions { get; set; }
    }

    public class NugutPackageVersionInfo
    {
        public NugutPackageVersionInfo()
        {
            Dependencies = new List<NugutPackageDependendcyInfo>();
        }

        public string Version { get; set; }
        public string TargetFramework { get; set; }

        public List<NugutPackageDependendcyInfo> Dependencies { get; set; }
    }

    public class NugutPackageDependendcyInfo
    {
        public string PackageName { get; set; }
        public string Version { get; set; }
    }


}
