using System;
using System.Xml;
using System.Xml.Linq;

namespace Intent.Modules.VisualStudio.Projects.NuGet
{
    public static class XmlNamespaceHelper
    {
        public static (string Prefix, XmlNamespaceManager NamespaceManager, string NamespaceName) GetNamespaceManager(this XDocument document)
        {
            if (document == null)
                throw new Exception("document is null");
            if (document.Root == null)
                throw new Exception("document.Root is null");

            const string prefix = "ns";
            var ns = document.Root.GetDefaultNamespace();
            var namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace(prefix, ns.NamespaceName);

            return (prefix, namespaceManager, ns.NamespaceName);
        }

    }
}
