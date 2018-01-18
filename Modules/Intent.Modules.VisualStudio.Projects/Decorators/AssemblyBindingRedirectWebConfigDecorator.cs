using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.VisualStudio;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Intent.Modules.VisualStudio.Projects.Decorators
{
    public class AssemblyBindingRedirectWebConfigDecorator : IWebConfigDecorator
    {
        public const string Identifier = "Intent.VisualStudio.Projects.AssemblyBindingRedirectWebConfig";
        private XmlNamespaceManager _namespaces;

        public AssemblyBindingRedirectWebConfigDecorator()
        {
        }

        public void Install(XDocument doc, IProject p)
        {
            _namespaces = new XmlNamespaceManager(new NameTable());
            var @namespace = doc.Root.GetDefaultNamespace();
            _namespaces.AddNamespace("ns", @namespace.NamespaceName);
            _namespaces.AddNamespace("urns", "urn:schemas-microsoft-com:asm.v1");

            var assemblyBinding = doc.XPathSelectElement("/ns:configuration/ns:runtime/urns:assemblyBinding", _namespaces);
            if (assemblyBinding == null)
            {
                assemblyBinding = new XElement("assemblyBinding");
                assemblyBinding.Add(new XAttribute("xmlns", "urn:schemas-microsoft-com:asm.v1"));

                AddAssemblyBindingRedirects(assemblyBinding, p.NugetPackages().SelectMany(x => x.AssemblyRedirects).ToList());
                doc.XPathSelectElement("/ns:configuration/ns:runtime", _namespaces).AddFirst(assemblyBinding);
            }
            else
            {
                AddAssemblyBindingRedirects(assemblyBinding, p.NugetPackages().SelectMany(x => x.AssemblyRedirects).ToList());
            }
        }

        private void AddAssemblyBindingRedirects(XElement assemblyBinding, List<AssemblyRedirectInfo> assemblyRedirects)
        {
            foreach (var assemblyRedirect in assemblyRedirects)
            {
                if (assemblyBinding.XPathSelectElement($"//urns:dependentAssembly//urns:assemblyIdentity[@name='{ assemblyRedirect.Name }']", _namespaces) != null)
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

                assemblyBinding.Add(dependentAssemly);
            }

        }
    }
}