﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.NuGet.Installer.HelperTypes;
using Intent.SoftwareFactory.Engine;
using NuGet.Versioning;

namespace Intent.Modules.NuGet.Installer.Schemes
{
    internal class VerboseWithPackageReferencesScheme : INuGetScheme
    {
        public Dictionary<string, SemanticVersion> GetInstalledPackages(IProject project, XNode xNode)
        {
            var packageReferenceElements = xNode.XPathSelectElements("Project/ItemGroup/PackageReference");
            return packageReferenceElements.ToDictionary(
                x => x.Attribute("Include")?.Value,
                x => SemanticVersion.Parse(x.Elements().Single(e => e.Name == "Version")?.Value));
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

                    packageReferenceItemGroup.Add(packageReferenceElement= new XElement("PackageReference",
                        new XAttribute("Include", packageId),
                        new XElement("Version", packageVersion)));
                }

                var versionElement = packageReferenceElement.Elements().SingleOrDefault(x => x.Name == "Version");
                if (versionElement == null)
                {
                    throw new Exception("Missing version element from PackageReference element.");
                }

                if (SemanticVersion.Parse(versionElement.Value) >= packageVersion)
                {
                    continue;
                }

                tracing.Info($"Upgrading {packageId} from {versionElement.Value} to {packageVersion} in project {project.ProjectName}");
                versionElement.SetValue(packageVersion);
            }

            // Remove and re-add the elements in alphabetical order
            var packageReferenceElements = packageReferenceItemGroup.Elements();
            foreach (var packageReferenceElement in packageReferenceElements.OrderBy(x => x.Attributes("Include").Single().Value))
            {
                packageReferenceElement.Remove();
                packageReferenceItemGroup.Add(packageReferenceElement);
            }
        }
    }
}
