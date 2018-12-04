using System;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.Common.VisualStudio;
using Intent.SoftwareFactory.Engine;

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
                .ToDictionary(x => x.Key, x => x);

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
                projectElement.Add("  ");
            }

            foreach (var addFileBehaviour in nugetPackages)
            {
                var latestVersion = addFileBehaviour.Value.OrderByDescending(x => x.Version).First().Version;
                var existingReference =
                    packageReferenceItemGroup.XPathSelectElement($"PackageReference[@Include='{addFileBehaviour.Key}']");

                if (existingReference == null)
                {
                    //tracing.Info($"{TracingOutputPrefix}Installing {addFileBehaviour.Key} {latestVersion} into project {netCoreProject.Name}");

                    packageReferenceItemGroup.Add("  ");
                    packageReferenceItemGroup.Add(new XElement("PackageReference",
                        new XAttribute("Include", addFileBehaviour.Key),
                        new XAttribute("Version", latestVersion)));
                    packageReferenceItemGroup.Add(Environment.NewLine);
                    packageReferenceItemGroup.Add("  ");
                }
            }
        }

        public static void SyncProjectReferences(this IProject _project, XDocument doc)
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