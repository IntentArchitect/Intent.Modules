using Intent.Modules.NuGet.Installer.NugetIntegration;
using System;
using IPackage = NuGet.IPackage;
using IVersionSpec = NuGet.IVersionSpec;

namespace Intent.Modules.NuGet.Installer.Managers
{
    public class Referer
    {
        private const string HIGHEST_SOLUTION_VERSION_REFERER_NAME = "Highest Solution Version";
        private const string LOWEST_SOLUTION_VERSION_REFERER_NAME = "Lowest Solution Version";

        public static Referer Create(string name, IVersionSpec versionSpec) => new Referer(name: name, packageNode: null, versionSpec: versionSpec);
        public static Referer Create(PackageNode packageNode, IVersionSpec versionSpec) => new Referer(name: null, packageNode: packageNode, versionSpec: versionSpec);
        public static Referer CreateFromSolutionHighestVersion(IPackage package) => new Referer(name: HIGHEST_SOLUTION_VERSION_REFERER_NAME, packageNode: null, versionSpec: package.Version.ToMinVersionSpecInclusive());
        public static Referer CreateFromSolutionLowestVersion(IPackage package) => new Referer(name: LOWEST_SOLUTION_VERSION_REFERER_NAME, packageNode: null, versionSpec: package.Version.ToMinVersionSpecInclusive());

        private Referer(string name, PackageNode packageNode, IVersionSpec versionSpec)
        {
            PackageNode = packageNode;
            VersionSpec = versionSpec ?? Utility.GetEmptyVersionSpec();
            Name = name ?? $"Package: {packageNode.RequiredPackage.Id}";
        }

        public PackageNode PackageNode { get; }
        public IVersionSpec VersionSpec { get; }
        public string Name { get; }

        public override string ToString()
        {
            return $"{Name} - {VersionSpec}";
        }

        #region Equals override and related
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Referer) obj);
        }

        protected bool Equals(Referer other)
        {
            return Equals(PackageNode, other.PackageNode) && Equals(VersionSpec, other.VersionSpec) && string.Equals(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (PackageNode != null ? PackageNode.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (VersionSpec != null ? VersionSpec.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(Name) : 0);
                return hashCode;
            }
        }
        #endregion
    }
}