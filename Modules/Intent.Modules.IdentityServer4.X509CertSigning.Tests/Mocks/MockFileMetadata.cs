using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.IdentityServer4.X509CertSigning.Tests.Mocks
{
    public class MockFileMetadata : IFileMetadata
    {
        public MockFileMetadata()
        {
        }

        public MockFileMetadata(string @namespace, string className)
        {
            CustomMetadata["Namespace"] = @namespace;
            CustomMetadata["ClassName"] = className;
        }

        public string CodeGenType { get; set; }

        public string FileExtension { get; set; }

        public OverwriteBehaviour OverwriteBehaviour { get; set; }

        public string FileName { get; set; }
        public string LocationInProject { get; set; }

        public IDictionary<string, string> CustomMetadata { get; set; } = new Dictionary<string, string>();

        public string GetFullLocationPath()
        {
            throw new NotImplementedException();
        }

        public string GetRelativeFilePath()
        {
            throw new NotImplementedException();
        }
    }
}
