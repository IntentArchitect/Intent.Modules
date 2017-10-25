using Intent.Modules.Bower.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.AngularJs.Auth.ImplicitAuth
{
    public static class BowerPackages
    {
        public static BowerPackageInfo JQuery = new BowerPackageInfo("jquery", "~2.2.4")
            .AddScriptRef("/dist/jquery.min.js")
            .InstallTypings();

        public static BowerPackageInfo Bootstrap = new BowerPackageInfo("bootstrap", "~3.3.6")
            .AddScriptRef("/dist/js/bootstrap.js")
            .AddCssRef("/dist/css/bootstrap.min.css")
            .InstallTypings()
            .AddDependency(JQuery);

        public static BowerPackageInfo Angular = new BowerPackageInfo("angular", "~1.5.7")
            .AddScriptRef("/angular.js")
            .InstallTypings()
            .AddDependency(JQuery);

        public static BowerPackageInfo AngularUiRouter = new BowerPackageInfo("angular-ui-router", "~0.3.1")
            .AddScriptRef("/release/angular-ui-router.js")
            .InstallTypings()
            .AddDependency(Angular);

        public static BowerPackageInfo OidcTokenManager = new BowerPackageInfo("oidc-token-manager", "~1.2.0")
            .AddScriptRef("/dist/oidc-token-manager.js")
            .InstallTypings()
            .AddDependency(Angular);

        public static BowerPackageInfo AngularLocalStorage = new BowerPackageInfo("angular-local-storage", "~0.2.4")
            .AddScriptRef("/dist/angular-local-storage.js")
            .InstallTypings()
            .AddDependency(Angular);

        public static BowerPackageInfo FontAwesome = new BowerPackageInfo("font-awesome", "~4.7.0")
            .AddCssRef("/css/font-awesome.min.css");
    }
}
