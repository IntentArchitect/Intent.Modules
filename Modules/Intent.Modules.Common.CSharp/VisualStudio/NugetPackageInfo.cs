using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.VisualStudio
{
    public class NugetPackageInfo : INugetPackageInfo, IEquatable<NugetPackageInfo>
    {
        public NugetPackageInfo(string name, string version)
        {
            Name = name;
            Version = version;
            AssemblyRedirects = new List<AssemblyRedirectInfo>();
        }

        public override string ToString()
        {
            return $"{Name}, {Version}";
        }

        public string Name { get; }
        public string Version { get; }
        public IList<AssemblyRedirectInfo> AssemblyRedirects { get; }
        public bool CanAddFile(string file)
        {
            switch (_addFileBlacklistBehaviour)
            {
                case AddFileBlacklistBehaviour.All:
                    return false;
                case AddFileBlacklistBehaviour.None:
                    return true;
                case AddFileBlacklistBehaviour.AllExcept:
                    return !_blacklistFileEntries.Any(x => x.Equals(file, StringComparison.OrdinalIgnoreCase));
                case AddFileBlacklistBehaviour.Only:
                    return _blacklistFileEntries.Any(x => x.Equals(file, StringComparison.OrdinalIgnoreCase));
                default:
                    throw new ArgumentOutOfRangeException(nameof(file));
            }
        }
        public string[] PrivateAssets { get; private set; }
        public string[] IncludeAssets { get; private set; }

        public NugetPackageInfo WithAssemblyRedirect(AssemblyRedirectInfo assemblyRedirect)
        {
            AssemblyRedirects.Add(assemblyRedirect);
            return this;
        }

        /// <summary>
        /// By default, all files allowed to be added.
        /// </summary>
        public NugetPackageInfo BlockAddingOfAllFiles()
        {
            _addFileBlacklistBehaviour = AddFileBlacklistBehaviour.All;
            _blacklistFileEntries = new string[0];
            return this;
        }

        /// <summary>
        /// By default, all files allowed to be added.
        /// </summary>
        public NugetPackageInfo AllowAddingOfAllFiles()
        {
            _addFileBlacklistBehaviour = AddFileBlacklistBehaviour.None;
            _blacklistFileEntries = new string[0];
            return this;
        }

        /// <summary>
        /// By default, all files allowed to be added.
        /// </summary>
        public NugetPackageInfo BlockAddingOfAllFilesExcept(IEnumerable<string> files)
        {
            _addFileBlacklistBehaviour = AddFileBlacklistBehaviour.Only;
            _blacklistFileEntries = files.ToArray();
            return this;
        }

        /// <summary>
        /// By default, all files allowed to be added.
        /// </summary>
        public NugetPackageInfo AllowAddingOfAllFilesExcept(IEnumerable<string> files)
        {
            _addFileBlacklistBehaviour = AddFileBlacklistBehaviour.AllExcept;
            _blacklistFileEntries = files.ToArray();
            return this;
        }

        public NugetPackageInfo SpecifyAssetsBehaviour(IEnumerable<string> privateAssets, IEnumerable<string> includeAssets)
        {
            PrivateAssets = privateAssets as string[] ?? privateAssets.ToArray();
            IncludeAssets = includeAssets as string[] ?? includeAssets.ToArray();
            
            return this;
        }


        private AddFileBlacklistBehaviour _addFileBlacklistBehaviour = AddFileBlacklistBehaviour.None;
        private string[] _blacklistFileEntries = new string[0];

        private enum AddFileBlacklistBehaviour
        {
            All = 0,
            None = 1,
            AllExcept = 2,
            Only = 3,
        }

        public bool Equals(NugetPackageInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NugetPackageInfo) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
