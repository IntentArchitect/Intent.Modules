using Intent.Modules.IdentityServer4.X509CertSigning.Templates.CertificateRepo;
using Intent.Modules.IdentityServer4.X509CertSigning.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace Intent.Modules.IdentityServer4.X509CertSigning.Tests
{
    public class CodeGeneratorTestInitializer
    {
        [Fact]
        public void GenerateCode()
        {
            string projectDir = GetProjectDirectory();

            var certificateRepo = new CertificateRepo(new MockOutputTarget(), null);
            certificateRepo.ConfigureFileMetadata(new MockFileMetadata(
                @namespace: typeof(CodeGeneratorTestInitializer).Namespace + ".GeneratedTemplates", 
                className: "CertificateRepo"));

            File.WriteAllText(
                Path.Combine(projectDir, "GeneratedTemplates", "CertificateRepo.cs"), 
                certificateRepo.TransformText());
        }

        private static string GetProjectDirectory()
        {
            var location = typeof(CodeGeneratorTestInitializer).Assembly.Location;
            var startIndex = location.IndexOf("\\bin");
            var projectDir = location.Remove(startIndex);
            return projectDir;
        }
    }
}
