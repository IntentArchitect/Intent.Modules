using System;
using System.Collections.Generic;
using System.Linq;
using IPackage = NuGet.IPackage;

namespace Intent.SoftwareFactory.NuGet.Managers
{
    public class PackageNode
    {
        public static PackageNode Create(IPackage package, Referer referer = null, IPackage installedPackage = null) => new PackageNode(package, installedPackage, referer);
        private PackageNode(IPackage requiredPackage, IPackage installedPackage, Referer referer)
        {
            Dependencies = new HashSet<PackageNode>();
            Referers = new HashSet<Referer>();
            RequiredPackage = requiredPackage;
            InstalledPackage = installedPackage;

            if (referer != null)
            {
                Referers.Add(referer);
            }
        }

        public IPackage RequiredPackage { get; private set; }

        public IPackage InstalledPackage { get; private set; }

        public ISet<PackageNode> Dependencies { get; }

        public ISet<Referer> Referers { get; }

        public void UpdateRequiredPackage(IPackage package)
        {
            if (package.Id != RequiredPackage.Id)
            {
                throw new Exception("Package Ids must match.");
            }

            if (package.Version < RequiredPackage.Version)
            {
                throw new Exception("Downgrading not allowed.");
            }

            RequiredPackage = package;
        }

        public void UpdateInstalledPackage(IPackage package)
        {
            if (InstalledPackage != null && package.Id != InstalledPackage.Id)
            {
                throw new Exception("Package Ids must match.");
            }

            if (InstalledPackage != null && package.Version < InstalledPackage.Version)
            {
                throw new Exception("Downgrading not allowed.");
            }

            InstalledPackage = package;
        }

        public void AddReferer(Referer referer)
        {
            var existingReferer = Referers.SingleOrDefault(x => x.PackageNode == referer.PackageNode && x.Name == referer.Name);
            if (existingReferer != null)
            {
                Referers.Remove(existingReferer);
            }

            Referers.Add(referer);
        }

        public bool IsInstalled()
        {
            return InstalledPackage?.Version == RequiredPackage.Version;
        }

        public bool SelfAndDependenciesInstalled()
        {
            return IsInstalled() && Dependencies.All(x => x.SelfAndDependenciesInstalled());
        }

        public bool IsDependentOn(IPackage package)
        {
            return Dependencies.Any(x => x.RequiredPackage.Id == package.Id || x.IsDependentOn(package));
        }
    }
}
