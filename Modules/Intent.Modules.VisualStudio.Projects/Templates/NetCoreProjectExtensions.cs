using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.Common.VisualStudio;
using Intent.Engine;

namespace Intent.Modules.VisualStudio.Projects.Templates
{
    public static class NetCoreProjectExtensions
    {
        public static void InstallNugetPackages(this IProject project, XDocument doc)
        {
            var nugetPackages = project
                .NugetPackages()
                .Distinct()
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => x)
                .Select(x => x.Value.OrderByDescending(y => y.Version).First())
                .OrderBy(x => x.Name)
                .ToArray();

            if (!nugetPackages.Any())
            {
                return;
            }

            var packageReferenceItemGroup = doc.XPathSelectElement("Project/ItemGroup[PackageReference]");
            if (packageReferenceItemGroup == null)
            {
                packageReferenceItemGroup = new XElement("ItemGroup");
                packageReferenceItemGroup.Add(Environment.NewLine);
                packageReferenceItemGroup.Add("  ");

                var projectElement = doc.XPathSelectElement("Project");

                projectElement.Add("  ");
                projectElement.Add(packageReferenceItemGroup);
                projectElement.Add(Environment.NewLine);
                projectElement.Add(Environment.NewLine);
            }

            foreach (var package in nugetPackages)
            {
                var existingReference =
                    packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{package.Name}']");


                // TODO: It would be nice if we inserted these alphatically into existing items
                if (existingReference == null)
                {
                    //tracing.Info($"{TracingOutputPrefix}Installing {nugetPackageInfo.Name} {nugetPackageInfo} into project {netCoreProject.Name}");

                    packageReferenceItemGroup.Add("  ");
                    var xElement = new XElement("PackageReference",
                        new XAttribute("Include", package.Name),
                        new XAttribute("Version", package.Version));
                    
                    var subElementAdded = false;
                    if (package.PrivateAssets != null && package.PrivateAssets.Any())
                    {
                        xElement.Add($"{Environment.NewLine}      ");
                        xElement.Add(new XElement("PrivateAssets", package.PrivateAssets.Aggregate((x, y) => x + "; " + y)));
                        subElementAdded = true;
                    }

                    if (package.IncludeAssets != null && package.IncludeAssets.Any())
                    {
                        xElement.Add($"{Environment.NewLine}      ");
                        xElement.Add(new XElement("PrivateAssets", package.IncludeAssets.Aggregate((x, y) => x + "; " + y)));
                        subElementAdded = true;
                    }

                    if (subElementAdded)
                    {
                        xElement.Add($"{Environment.NewLine}    ");
                    }

                    packageReferenceItemGroup.Add(xElement);
                    packageReferenceItemGroup.Add(Environment.NewLine);
                    packageReferenceItemGroup.Add("  ");
                }
            }
        }

        public static void SyncProjectReferences(this IOutputTarget _project, XDocument doc)
        {
            if (_project.Dependencies().Count <= 0)
            {
                return;
            }

            var itemGroupElement = doc.XPathSelectElement("Project/ItemGroup[ProjectReference]");
            if (itemGroupElement == null)
            {
                itemGroupElement = new XElement("ItemGroup");
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");

                var projectElement = doc.XPathSelectElement("Project");

                projectElement.Add("  ");
                projectElement.Add(itemGroupElement);
                projectElement.Add(Environment.NewLine);
                projectElement.Add("  ");
            }

            foreach (var dependency in _project.Dependencies())
            {
                var projectUrl = string.Format("..\\{0}\\{0}.csproj", dependency.Name);
                var projectReferenceItem = doc.XPathSelectElement($"/Project/ItemGroup/ProjectReference[@Include='{projectUrl}']");
                if (projectReferenceItem != null)
                {
                    continue;
                }

                /*
                <ProjectReference Include="..\Intent.SoftwareFactory\Intent.SoftwareFactory.csproj"/>
                */

                var item = new XElement(XName.Get("ProjectReference"));
                item.Add(new XAttribute("Include", projectUrl));

                itemGroupElement.Add("  ");
                itemGroupElement.Add(item);
                itemGroupElement.Add(Environment.NewLine);
                itemGroupElement.Add("  ");
            }
        }
    }
}