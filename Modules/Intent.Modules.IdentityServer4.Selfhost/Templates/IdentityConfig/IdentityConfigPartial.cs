using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.Selfhost.Templates.IdentityConfig
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class IdentityConfig : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "IdentityServer4.Selfhost.IdentityConfig";

        public IdentityConfig(IOutputTarget outputTarget, object model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(NugetPackages.IdentityServer4);
        }

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"IdentityConfig",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

    }
}