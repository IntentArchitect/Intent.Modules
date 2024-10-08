﻿using System.Collections.Generic;

namespace Intent.Modules.Common.VisualStudio
{
    public interface INugetPackageInfo
    {
        string Name { get; }
        string Version { get; }
        bool CanAddFile(string file);
        /// <summary>
        /// Used for Web.config to set up Assembly Redirects.
        /// </summary>
        IList<AssemblyRedirectInfo> AssemblyRedirects { get; }
        string[] PrivateAssets { get; }
        string[] IncludeAssets { get; }

        /// <summary>
        /// List of NuGetpackages this package installs / is dependent on
        /// </summary>
        IList<INugetPackageDependency>  Dependencies { get; }
    }
}
