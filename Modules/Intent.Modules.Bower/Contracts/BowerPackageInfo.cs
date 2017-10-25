using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Bower.Contracts
{
    public class BowerPackageInfo : IBowerPackageInfo
    {
        /*
        [Obsolete("Specific Bower packages and their versions cannot live in the SDK")]
        public static BowerPackageInfo JQuery = new BowerPackageInfo("jquery", "~2.2.4")
            .AddScriptRef("/dist/jquery.min.js")
            .InstallTypings();
        [Obsolete("Specific Bower packages and their versions cannot live in the SDK")]
        public static BowerPackageInfo Bootstrap = new BowerPackageInfo("bootstrap", "~3.3.6")
            .AddScriptRef("/dist/js/bootstrap.js")
            .AddCssRef("/dist/css/bootstrap.min.css")
            .InstallTypings()
            .AddDependency(JQuery);
        [Obsolete("Specific Bower packages and their versions cannot live in the SDK")]
        public static BowerPackageInfo Angular = new BowerPackageInfo("angular", "~1.5.7")
            .AddScriptRef("/angular.js")
            .InstallTypings()
            .AddDependency(JQuery);
        [Obsolete("Specific Bower packages and their versions cannot live in the SDK")]
        public static BowerPackageInfo AngularUiRouter = new BowerPackageInfo("angular-ui-router", "~0.3.1")
            .AddScriptRef("/release/angular-ui-router.js")
            .InstallTypings()
            .AddDependency(Angular);
        [Obsolete("Specific Bower packages and their versions cannot live in the SDK")]
        public static BowerPackageInfo OidcTokenManager = new BowerPackageInfo("oidc-token-manager", "~1.2.0")
            .AddScriptRef("/dist/oidc-token-manager.js")
            .InstallTypings()
            .AddDependency(Angular);
        [Obsolete("Specific Bower packages and their versions cannot live in the SDK")]
        public static BowerPackageInfo AngularLocalStorage = new BowerPackageInfo("angular-local-storage", "~0.2.4")
            .AddScriptRef("/dist/angular-local-storage.js")
            .InstallTypings()
            .AddDependency(Angular);
        [Obsolete("Specific Bower packages and their versions cannot live in the SDK")]
        public static BowerPackageInfo FontAwesome = new BowerPackageInfo("font-awesome", "~4.7.0")
            .AddCssRef("/css/font-awesome.min.css");

    */
        public BowerPackageInfo(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; }
        public string Version { get; }
        public bool InstallsTypings { get; set; }
        public string TypingsName { get; set; }
        public string TypingsLocation { get; set; }
        public IList<string> JsSources { get; } = new List<string>();
        public IList<string> CssSources { get; } = new List<string>();
        public IList<IBowerPackageInfo> Dependencies { get; } = new List<IBowerPackageInfo>();
        public int GetFullDependencyCount()
        {
            var count = 0;
            foreach (var dependency in Dependencies)
            {
                count += dependency.GetFullDependencyCount();
                count++;
            }
            return count;
        }

        public BowerPackageInfo AddScriptRef(string relativePath)
        {
            JsSources.Add(Name + relativePath);
            return this;
        }

        public BowerPackageInfo AddCssRef(string relativePath)
        {
            CssSources.Add(Name + relativePath);
            return this;
        }

        public BowerPackageInfo InstallTypings(string typingsName = null, string typingsLocation = null)
        {
            InstallsTypings = true;
            TypingsName = typingsName ?? Name;
            TypingsLocation = typingsName;
            return this;
        }

        public BowerPackageInfo AddDependency(BowerPackageInfo dependency)
        {
            Dependencies.Add(dependency);
            return this;
        }

        public bool Equals(IBowerPackageInfo other)
        {
            if (other == null)
                return false;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is IBowerPackageInfo))
                return false;

            return Equals((IBowerPackageInfo)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
