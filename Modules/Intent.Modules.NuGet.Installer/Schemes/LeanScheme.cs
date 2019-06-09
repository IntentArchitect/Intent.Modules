using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.SoftwareFactory.Engine;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer.Schemes
{
    internal class LeanScheme : INuGetScheme
    {
        public Dictionary<string, SemanticVersion> GetInstalledPackages(IProject project, XNode xNode)
        {
            var packageReferenceElements = xNode.XPathSelectElements("Project/ItemGroup/PackageReference");
            return packageReferenceElements.ToDictionary(
                x => x.Attribute("Include")?.Value,
                x => SemanticVersion.Parse(x.Attribute("Version")?.Value));
        }

        public void InstallPackages(NuGetProject project, ITracing tracing)
        {
            var packageReferenceItemGroup = project.Document.XPathSelectElement("Project/ItemGroup[PackageReference]");
            if (packageReferenceItemGroup == null)
            {
                var projectElement = project.Document.XPathSelectElement("Project");
                if (projectElement == null)
                {
                    throw new Exception("Project element not found.");
                }

                projectElement.Add(packageReferenceItemGroup = new XElement("ItemGroup"));
            }

            foreach (var requestedPackage in project.RequestedPackages)
            {
                var packageId = requestedPackage.Key;
                var packageVersion = requestedPackage.Value;

                var packageReferenceElement = packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{packageId}']");
                if (packageReferenceElement == null)
                {
                    tracing.Info($"Installing {packageId} {packageVersion} into project {project.ProjectName}");
                    packageReferenceItemGroup.Add(packageReferenceElement = new XElement("PackageReference",
                        new XAttribute("Include", packageId), 
                        new XAttribute("Version", packageVersion)));
                }

                var versionAttribute = packageReferenceElement.Attributes().SingleOrDefault(x => x.Name == "Version");
                if (versionAttribute == null)
                {
                    throw new Exception("Missing version attribute from PackageReference element.");
                }

                if (SemanticVersion.Parse(versionAttribute.Value) >= packageVersion)
                {
                    continue;
                }

                tracing.Info($"Upgrading {packageId} from {versionAttribute.Value} to {packageVersion} in project {project.ProjectName}");
                versionAttribute.SetValue(packageVersion);
            }
        }
    }
}
