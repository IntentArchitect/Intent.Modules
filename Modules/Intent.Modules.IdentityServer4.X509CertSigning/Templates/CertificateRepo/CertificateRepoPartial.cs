using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.IdentityServer4.X509CertSigning.Templates.CertificateRepo
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class CertificateRepo : CSharpTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "IdentityServer4.X509CertSigning.CertificateRepo";

        public CertificateRepo(IOutputTarget outputTarget, object model) : base(TemplateId, outputTarget, model)
        {
        }

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"CertificateRepo",
                @namespace: $"{OutputTarget.GetNamespace()}");
        }

    }
}