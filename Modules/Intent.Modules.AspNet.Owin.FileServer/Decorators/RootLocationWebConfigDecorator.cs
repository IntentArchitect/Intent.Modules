using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.VSProjects.Decorators;// This dependency is not ideal

namespace Intent.Packages.Owin.FileServer.Decorators
{
    public class RootLocationWebConfigDecorator : IWebConfigDecorator
    {
        public const string Identifier = "Intent.Owin.FileServer.WebApiConfiguration";
        public RootLocationWebConfigDecorator()
        {
        }

        public void Install(XDocument doc, IProject project)
        {
            var namespaces = new XmlNamespaceManager(new NameTable());
            var _namespace = doc.Root.GetDefaultNamespace();
            namespaces.AddNamespace("ns", _namespace.NamespaceName);

            var webServerHandlers = doc.XPathSelectElement("/ns:configuration/ns:system.webServer/ns:handlers", namespaces);
            if (webServerHandlers.XPathSelectElement("//add[@name='Owin']", namespaces) == null)
            {
                AddOwinWebServerHandler(webServerHandlers);
            }
        }

        private static void AddOwinWebServerHandler(XElement webServerHandlers)
        {
            var owinWebServerHandler = new XElement("add");
            owinWebServerHandler.Add(new XAttribute("name", "Owin"));
            owinWebServerHandler.Add(new XAttribute("verb", ""));
            owinWebServerHandler.Add(new XAttribute("path", "*"));
            owinWebServerHandler.Add(new XAttribute("type", "Microsoft.Owin.Host.SystemWeb.OwinHttpHandler, Microsoft.Owin.Host.SystemWeb"));
            webServerHandlers.Add(owinWebServerHandler);
        }
    }
}