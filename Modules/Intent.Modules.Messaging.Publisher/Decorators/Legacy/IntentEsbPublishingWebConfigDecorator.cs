using System.Collections;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.VSProjects.Decorators;

namespace Intent.Packages.Messaging.Publisher.Decorators.Legacy
{
    public class IntentEsbPublishingWebConfigDecorator : IWebConfigDecorator
    {
        public const string Identifier = "Intent.Messaging.Publisher.WebConfigDecorator.Legacy";
        public void Install(XDocument doc, IProject project)
        {
            var configSections = ((IEnumerable)doc.XPathEvaluate("//configuration//configSections")).Cast<XElement>().First();
            if (!((IEnumerable)configSections.XPathEvaluate("//section[@name='akka']")).Cast<XElement>().Any())
            {
                var akkaSection = new XElement("section");
                akkaSection.Add(new XAttribute("name", "akka"));
                akkaSection.Add(new XAttribute("type", "Akka.Configuration.Hocon.AkkaConfigurationSection, Akka"));
                configSections.Add(akkaSection);
            }

            if (!((IEnumerable)doc.XPathEvaluate("//configuration//akka")).Cast<XElement>().Any())
            {
                var element = XElement.Parse(
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
                var systemDotWeb = ((IEnumerable)doc.XPathEvaluate("//configuration//system.web")).Cast<XElement>().First();
                systemDotWeb.AddAfterSelf(element);
            }
        }
    }
}