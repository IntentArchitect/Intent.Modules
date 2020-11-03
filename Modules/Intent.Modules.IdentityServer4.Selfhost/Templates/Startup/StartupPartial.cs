using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Selfhost.Templates.Startup
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class Startup : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "IdentityServer4.Selfhost.Startup";

        public Startup(IOutputTarget outputTarget, object model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.IdentityServer4);
        }

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"Startup",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

        public string GetClientsConfiguration()
        {
            return $"{GetTemplateClassName(IdentityConfig.IdentityConfig.TemplateId)}.Clients";
        }

        public string GetApiResourcesConfiguration()
        {
            return $"{GetTemplateClassName(IdentityConfig.IdentityConfig.TemplateId)}.ApiResources";
        }

        public string GetScopesConfiguration()
        {
            return $"{GetTemplateClassName(IdentityConfig.IdentityConfig.TemplateId)}.Scopes";
        }

        public string GetIdentityResourcesConfiguration()
        {
            return $"{GetTemplateClassName(IdentityConfig.IdentityConfig.TemplateId)}.IdentityResources";
        }

    }
}