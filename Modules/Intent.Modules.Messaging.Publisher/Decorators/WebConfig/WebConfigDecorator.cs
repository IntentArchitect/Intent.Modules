using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory.Engine;

namespace Intent.Modules.Messaging.Publisher.Decorators.WebConfig
{
    public class WebConfigDecorator : IWebConfigDecorator
    {
        public const string IDENTIFIER = "Intent.Messaging.Publisher.WebConfigDecorator";
        public void Install(XDocument doc, IProject project)
        {
            // We no longer use Akka, so remove it if it's in there.
            if (doc == null) throw new Exception("doc is null");
            if (doc.Root == null) throw new Exception("doc.Root is null");

            var namespaces = new XmlNamespaceManager(new NameTable());
            namespaces.AddNamespace("ns", doc.Root.GetDefaultNamespace().NamespaceName);

            var configSectionAkkaElement = doc.XPathSelectElement("/ns:configuration/ns:configSections/ns:section[@name='akka']", namespaces);
            configSectionAkkaElement?.Remove();
            
            var akkaElement = doc.XPathSelectElement("/ns:configuration//akka", namespaces);
            akkaElement?.Remove();
        }
    }
}