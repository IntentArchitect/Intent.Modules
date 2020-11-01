using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.Decorators
{
    public class AssemblyBindingRedirectWebConfigDecorator : IWebConfigDecorator
    {
        public const string IDENTIFIER = "Intent.VisualStudio.Projects.AssemblyBindingRedirectWebConfig";
        private XmlNamespaceManager _namespaces;

        public int Priority { get; } = 0;

        public void Install(XDocument doc, IOutputTarget p)
        {
            _namespaces = new XmlNamespaceManager(new NameTable());
            var defaultNamespace = doc.Root.GetDefaultNamespace();
            _namespaces.AddNamespace("ns", defaultNamespace.NamespaceName);
            _namespaces.AddNamespace("urns", "urn:schemas-microsoft-com:asm.v1");

            var configurationElement = doc.XPathSelectElement("/ns:configuration", _namespaces);
            if (configurationElement == null)
            {
                doc.Add(configurationElement = new XElement("configuration"));
            }

            var runtimeElement = doc.XPathSelectElement("/ns:configuration/ns:runtime", _namespaces);
            if (runtimeElement == null)
            {
                configurationElement.Add(runtimeElement = new XElement("runtime"));
            }

            var assemblyBindingElement = doc.XPathSelectElement("/ns:configuration/ns:runtime/urns:assemblyBinding", _namespaces);
            if (assemblyBindingElement == null)
            {
                runtimeElement.AddFirst(assemblyBindingElement = new XElement((XNamespace)_namespaces.LookupNamespace("urns") + "assemblyBinding"));
            }

            var nugetPackages = p.Application.OutputTargets.Where(x => x.IsVSProject()).SelectMany(x => x.NugetPackages()).Distinct(); // NOTE: This is not very robust as we are "distincting" on name. Logic should be implemented to choose which version.
            AddAssemblyBindingRedirects(assemblyBindingElement, nugetPackages.SelectMany(x => x.AssemblyRedirects).ToList());
        }

        private void AddAssemblyBindingRedirects(XElement assemblyBindingElement, List<AssemblyRedirectInfo> assemblyRedirects)
        {
            foreach (var assemblyRedirect in assemblyRedirects)
            {
                if (assemblyBindingElement.XPathSelectElement($"//urns:dependentAssembly//urns:assemblyIdentity[@name='{ assemblyRedirect.Name }']", _namespaces) != null)
                {
                    continue;
                }

                XNamespace @namespace = _namespaces.LookupNamespace("urns");
                var dependentAssemly = new XElement(@namespace + "dependentAssembly");

                var assemblyIdentity = new XElement(@namespace + "assemblyIdentity");
                assemblyIdentity.Add(new XAttribute("name", assemblyRedirect.Name));
                assemblyIdentity.Add(new XAttribute("publicKeyToken", assemblyRedirect.PublicKey));
                dependentAssemly.Add(assemblyIdentity);

                var bindingRedirect = new XElement(@namespace + "bindingRedirect");
                bindingRedirect.Add(new XAttribute("oldVersion", $"0.0.0.0-{ assemblyRedirect.Version }"));
                bindingRedirect.Add(new XAttribute("newVersion", assemblyRedirect.Version));
                dependentAssemly.Add(bindingRedirect);

                assemblyBindingElement.Add(dependentAssemly);
            }

        }
    }
}