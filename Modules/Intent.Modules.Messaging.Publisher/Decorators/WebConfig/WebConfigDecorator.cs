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
            if (doc == null) throw new Exception("doc is null");
            if (doc.Root == null) throw new Exception("doc.Root is null");

            var namespaces = new XmlNamespaceManager(new NameTable());
            namespaces.AddNamespace("ns", doc.Root.GetDefaultNamespace().NamespaceName);

            var configurationElement = doc.XPathSelectElement("/ns:configuration", namespaces);
            if (configurationElement == null)
            {
                doc.Add(configurationElement = new XElement("configuration"));
            }

            var configSectionsElement = doc.XPathSelectElement("/ns:configuration/ns:configSections", namespaces);
            if (configSectionsElement == null)
            {
                configurationElement.Add(configSectionsElement = new XElement("configSections"));
            }

            var configSectionAkkaElement = doc.XPathSelectElement("/ns:configuration/ns:configSections/ns:section[@name='akka']", namespaces);
            if (configSectionAkkaElement == null)
            {
                configSectionAkkaElement = new XElement("section");
                configSectionAkkaElement.Add(new XAttribute("name", "akka"));
                configSectionAkkaElement.Add(new XAttribute("type", "Akka.Configuration.Hocon.AkkaConfigurationSection, Akka"));
                configSectionsElement.Add(configSectionAkkaElement);
            }

            var akkaElement = doc.XPathSelectElement("/ns:configuration//akka", namespaces);
            if (akkaElement != null)
            {
                return;
            }

            akkaElement = XElement.Parse(
                @"<akka>
    <hocon><![CDATA[
akka {
    stdout-loglevel = DEBUG
    loglevel = DEBUG
    log-config-on-start = on
    loggers = [""Akka.Logger.NLog.NLogLogger, Akka.Logger.NLog""]
    actor {
        provider = ""Akka.Remote.RemoteActorRefProvider, Akka.Remote""
        debug {
              receive = on
              autoreceive = on
              lifecycle = on
              event-stream = on
              unhandled = on
        }
    }
    remote {
        helios.tcp {
            transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
            applied-adapters = []
            transport-protocol = tcp
            port = 0
            hostname = localhost
        }
        log-remote-lifecycle-events = DEBUG
    }
}
      ]]></hocon>
  </akka>");

            var systemWebElement = doc.XPathSelectElement("/ns:configuration/ns:system.web", namespaces);
            if (systemWebElement != null)
            {
                systemWebElement.AddAfterSelf(akkaElement);
            }
            else
            {
                configurationElement.Add(akkaElement);
            }
        }
    }
}