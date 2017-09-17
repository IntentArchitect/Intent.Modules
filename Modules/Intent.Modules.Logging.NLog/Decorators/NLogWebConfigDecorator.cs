using Intent.Modules.VisualStudio.Projects.Decorators;
using Intent.SoftwareFactory.Engine;
using System;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Intent.Modules.Logging.NLog.Decorators
{
    public class NLogWebConfigDecorator : IWebConfigDecorator
    {
        public const string Identifier = "Intent.Logging.NLog.WebConfigDecorator";

        public void Install(XDocument doc, IProject project)
        {
            SetupConfigSectionElement(doc);
            SetupNlogElement(doc, project.ApplicationName());
        }

        private static void SetupConfigSectionElement(XDocument doc)
        {
            var namespaces = new XmlNamespaceManager(new NameTable());
            var _namespace = doc.Root.GetDefaultNamespace();
            namespaces.AddNamespace("ns", _namespace.NamespaceName);

            var configurationElement = doc.XPathSelectElement("/ns:configuration", namespaces);
            if (configurationElement == null)
            {
                throw new Exception("Configuration element missing.");
            }

            var configSectionsElement = doc.XPathSelectElement("/ns:configuration/ns:configSections", namespaces);
            if (configSectionsElement == null)
            {
                configSectionsElement = new XElement("configSections");
                configurationElement.AddFirst(configSectionsElement);
            }

            if (configSectionsElement.XPathSelectElement("//section[@name='nlog']", namespaces) == null)
            {
                configSectionsElement.Add(new XElement(
                    "section",
                    new XAttribute("name", "nlog"),
                    new XAttribute("type", "NLog.Config.ConfigSectionHandler, NLog")));
            }
        }

        private static void SetupNlogElement(XDocument doc, string folderName)
        {
            var namespaces = new XmlNamespaceManager(new NameTable());
            var _namespace = doc.Root.GetDefaultNamespace();
            namespaces.AddNamespace("nlog", "http://www.nlog-project.org/schemas/NLog.xsd");
            namespaces.AddNamespace("ns", _namespace.NamespaceName);

            var configurationElement = doc.XPathSelectElement("/ns:configuration", namespaces);
            if (configurationElement == null)
            {
                throw new Exception("Configuration element missing.");
            }

            var nlogElement = doc.XPathSelectElement("/ns:configuration/nlog:nlog", namespaces);
            if (nlogElement == null)
            {
                configurationElement.Add(XElement.Parse($@"
                    <nlog xmlns=""http://www.nlog-project.org/schemas/NLog.xsd"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">
                      <!--
                      If NLog isn't writing to a log file, add the following attribute to the nlog element to get more diagnostic info logged:
                      internalLogFile=""nLogInternalLogFile.txt""
                      -->
                        <targets>
                          <target name=""debugger"" xsi:type=""Debugger"" layout=""${{longdate}}|${{level:uppercase=true}}|${{logger}}|${{operation-context-id}}|${{message}}"" />
                          <target name=""asyncFile"" type=""AsyncWrapper"" overflowAction=""Block"" queueLimit=""1000000"" batchSize=""1000"" timeToSleepBetweenBatches=""1000"">
                            <target type=""File"" fileName=""C:/Diagnostics/{folderName}/Log.txt"" archiveFileName=""C:/Diagnostics/{folderName}/Log.{{#}}.txt"" archiveDateFormat=""yyyyMMdd"" archiveNumbering=""DateAndSequence"" archiveAboveSize=""33554432"" maxArchiveFiles=""8"" autoFlush=""false"" keepFileOpen=""true"" layout=""${{longdate}}|${{level:uppercase=true}}|${{logger}}|${{operation-context-id}}|${{message}}"" />
                          </target>
                        </targets>
                        <rules>
                          <!-- There are performance implications to have the debugger target enabled. -->
                          <!--<logger minlevel=""Trace"" writeTo=""debugger"" />-->
                          <logger minlevel=""Trace"" writeTo=""asyncFile"" />
                        </rules>
                    </nlog>"));
            }
        }
    }
}