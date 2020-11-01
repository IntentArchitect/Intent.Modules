using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Modules.VisualStudio.Projects.NuGet.HelperTypes;
using NuGet.Versioning;

namespace Intent.Modules.VisualStudio.Projects.NuGet.SchemeProcessors
{
    internal class VerboseWithPackageReferencesSchemeProcessor : INuGetSchemeProcessor
    {
        public Dictionary<string, NuGetPackage> GetInstalledPackages(string projectPath, XNode xNode)
        {
            var (prefix, namespaceManager, _) = xNode.Document.GetNamespaceManager();

            var packageReferenceElements = xNode.XPathSelectElements($"/{prefix}:Project/{prefix}:ItemGroup/{prefix}:PackageReference", namespaceManager);
            return packageReferenceElements.ToDictionary(
                element => element.Attribute("Include")?.Value,
                element =>
                {
                    var version = element.XPathSelectElement($"{prefix}:Version", namespaceManager)?.Value ??
                        element.Attributes("Version").FirstOrDefault()?.Value;

                    var privateAssetsElement = element.XPathSelectElement($"{prefix}:PrivateAssets", namespaceManager);
                    var privateAssets = privateAssetsElement != null
                        ? privateAssetsElement.Value
                            .Split(';')
                            .Select(y => y.Trim())
                        : new string[0];

                    var includeAssetsElement = element.XPathSelectElement($"{prefix}:IncludeAssets", namespaceManager);
                    var includeAssets = includeAssetsElement != null
                        ? includeAssetsElement.Value
                            .Split(';')
                            .Select(y => y.Trim())
                        : new string[0];

                    return NuGetPackage.Create(version, includeAssets, privateAssets);
                });
        }

        public string InstallPackages(
            string projectContent,
            Dictionary<string, NuGetPackage> requestedPackages,
            Dictionary<string, NuGetPackage> installedPackages,
            string projectName,
            ITracing tracing)
        {
            var document = XDocument.Parse(projectContent);
            var (prefix, namespaceManager, namespaceName) = document.GetNamespaceManager();

            var packageReferenceItemGroup = document.XPathSelectElement($"/{prefix}:Project/{prefix}:ItemGroup[{prefix}:PackageReference]", namespaceManager);

            // Try add after last ItemGroup, otherwise last PropertyGroup
            if (packageReferenceItemGroup == null)
            {
                var element = document.XPathSelectElements($"/{prefix}:Project/{prefix}:ItemGroup", namespaceManager).LastOrDefault() ??
                              document.XPathSelectElements($"/{prefix}:Project/{prefix}:PropertyGroup", namespaceManager).LastOrDefault();
                element?.AddAfterSelf(packageReferenceItemGroup = new XElement(XName.Get("ItemGroup", namespaceName)));
            }

            // Otherwise, add to Project element
            if (packageReferenceItemGroup == null)
            {
                var projectElement = document.XPathSelectElement($"/{prefix}:Project", namespaceManager);
                if (projectElement == null)
                {
                    throw new Exception("Project element not found.");
                }

                projectElement.AddFirst(packageReferenceItemGroup = new XElement(XName.Get("ItemGroup", namespaceName)));
            }

            foreach (var requestedPackage in requestedPackages)
            {
                var packageId = requestedPackage.Key;
                var package = requestedPackage.Value;

                var packageReferenceElement = packageReferenceItemGroup.XPathSelectElement($"{prefix}:PackageReference[@Include='{packageId}']", namespaceManager);
                if (packageReferenceElement == null)
                {
                    tracing.Info($"Installing {packageId} {package.Version} into project {projectName}");

                    packageReferenceItemGroup.Add(packageReferenceElement = new XElement(
                        XName.Get("PackageReference", namespaceName),
                        new XAttribute("Include", packageId),
                        new XElement(XName.Get("Version", namespaceName), package.Version.OriginalString)));
                }

                var versionElement = packageReferenceElement.XPathSelectElement($"{prefix}:Version", namespaceManager); // try element first
                var versionAttribute = packageReferenceElement.Attributes("Version").FirstOrDefault(); // could be attribute
                if (versionElement == null && versionAttribute == null)
                {
                    throw new Exception("Missing version element from PackageReference element.");
                }

                if (VersionRange.Parse(versionElement?.Value ?? versionAttribute.Value).MinVersion >= package.Version.MinVersion)
                {
                    continue;
                }

                tracing.Info($"Upgrading {packageId} from {versionElement?.Value ?? versionAttribute.Value} to {package.Version} in project {projectName}");
                versionElement?.SetValue(package.Version.OriginalString);
                versionAttribute?.SetValue(package.Version.OriginalString);
            }

            SortAlphabetically(packageReferenceItemGroup);
            return Format(document);
        }

        private static void SortAlphabetically(XElement packageReferenceItemGroup)
        {
            // Remove and re-add the elements in alphabetical order
            var packageReferenceElements = packageReferenceItemGroup
                .Elements()
                .OrderBy(x => x.Attributes("Include").Single().Value);

            foreach (var packageReferenceElement in packageReferenceElements)
            {
                packageReferenceElement.Remove();
                packageReferenceItemGroup.Add(packageReferenceElement);
            }
        }

        private static string Format(XDocument document)
        {
            document = XDocument.Parse(document.ToString(), LoadOptions.PreserveWhitespace);
            if (document == null)
                throw new Exception("document is null");
            if (document.Root == null)
                throw new Exception("document.Root is null");

            FormatNodes(document.Root.Nodes());

            return $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{document}";
        }

        private static void FormatNodes(IEnumerable<XNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (!(node is XElement element))
                {
                    continue;
                }

                if (element.HasElements)
                {
                    FormatNodes(element.Nodes());
                    continue;
                }

                if (!element.IsEmpty && element.Value == string.Empty)
                {
                    if (!(element.PreviousNode is XText previousNode))
                    {
                        throw new Exception("Expected previous node to whitespace between elements");
                    }

                    element.Add(new XText(previousNode.Value));
                }
            }
        }
    }
}
