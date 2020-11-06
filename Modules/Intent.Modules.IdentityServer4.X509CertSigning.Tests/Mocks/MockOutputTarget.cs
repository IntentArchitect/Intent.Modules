using Intent.Engine;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Intent.Modules.IdentityServer4.X509CertSigning.Tests.Mocks
{
    public class MockOutputTarget : IOutputTarget
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Location { get; set; }

        public string RelativeLocation { get; set; }

        public string Type { get; set; }

        public IOutputTarget Parent { get; set; }

        public IEnumerable<ITemplate> TemplateInstances { get; set; }

        public IDictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        public IApplication Application { get; set; }

        public ISoftwareFactoryExecutionContext ExecutionContext { get; set; }

        public ITemplateTargetInfo AsTarget(string subLocation = null)
        {
            throw new NotImplementedException();
        }

        public bool Equals([AllowNull] IOutputTarget other)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetSupportedFrameworks()
        {
            throw new NotImplementedException();
        }

        public IList<IOutputTarget> GetTargetPath()
        {
            throw new NotImplementedException();
        }

        public bool HasRole(string role)
        {
            throw new NotImplementedException();
        }

        public bool HasTemplateInstances(string templateId)
        {
            throw new NotImplementedException();
        }

        public bool HasTemplateInstances(string templateId, Func<ITemplate, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public bool OutputsTemplate(string templateId)
        {
            throw new NotImplementedException();
        }
    }
}
