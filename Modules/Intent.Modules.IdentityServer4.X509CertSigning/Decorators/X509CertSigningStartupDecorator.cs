using Intent.Engine;
using Intent.Modules.Common;
using Intent.Modules.Common.IdentityServer4.Decorators;
using Intent.Modules.IdentityServer4.X509CertSigning.Templates.CertificateRepo;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.InMemoryStore.Decorators
{
    public class X509CertSigningStartupDecorator : StartupDecorator
    {
        public const string Identifier = "IdentityServer4.X509CertSigning.StartupDecorator";

        public override int Priority => 1;

        public override IReadOnlyCollection<string> GetServicesConfigurationStatements()
        {
            return new[] 
            {
                @"AddSigningCredential(CertificateRepo.GetFromFile(""path to pfx file""))"
            };
        }
    }
}
