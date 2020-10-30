using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Eventing;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    public class FrameworkProjectSyncProcessor
    {
        private readonly IXmlFileCache _xmlFileCache;
        private readonly IChanges _changeManager;
        private readonly string _projectPath;
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IOutputTarget _project;

        private Action<string, string> _syncProjectFile;
        private XDocument _doc;
        private XmlNamespaceManager _namespaces;
        private XNamespace _namespace;
        private XElement _projectElement;

        public FrameworkProjectSyncProcessor(
            string projectPath,
            ISoftwareFactoryEventDispatcher sfEventDispatcher,
            IXmlFileCache xmlFileCache,
            IChanges changeManager,
            IOutputTarget project)
        {
            _projectPath = projectPath;
            _sfEventDispatcher = sfEventDispatcher;
            _xmlFileCache = xmlFileCache;
            _changeManager = changeManager;
            _project = project;
            _syncProjectFile = UpdateFileOnHdd;

        }

        public void Process(List<SoftwareFactoryEvent> events)
        {
            var filename = LoadProjectFile();

            if (string.IsNullOrWhiteSpace(filename))
                return;

            filename = Path.GetFullPath(filename);

            //run events
            ProcessEvents(events);
            SyncAssemblyReferences();
            SyncProjectReferences();

            var currentProjectFileContent = "";
            if (File.Exists(filename))
            {
                var currentProjectFile = XDocument.Parse(File.ReadAllText(filename));
                currentProjectFileContent = currentProjectFile.ToString();
            }

            // Re-parsing prevents issue where not all elements get created on a new line.
            var outputContent = XDocument.Parse(_doc.ToString()).ToString();

            //trying to do a schemantic comparision as VS does inconsistence formatting 
            if (currentProjectFileContent != outputContent)
            {
                Logging.Log.Debug($"Syncing changes to Project File {filename}");
                //string content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + outputContent;

                _syncProjectFile(filename, $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}{outputContent}");
            }
        }

        public void ProcessEvents(List<SoftwareFactoryEvent> events)
        {
            foreach (var @event in events)
            {
                switch (@event.EventIdentifier)
                {
                    case SoftwareFactoryEvents.FileAdded:
                        ProcessAddProjectItem(
                            path: @event.GetValue("Path"),
                            itemType: @event.TryGetValue("ItemType"),
                            dependsOn: @event.TryGetValue("Depends On"),
                            copyToOutputDirectory: @event.TryGetValue("CopyToOutputDirectory"));
                        break;
                    case SoftwareFactoryEvents.FileRemoved:
                        ProcessRemoveProjectItem(
                            path: @event.GetValue("Path"));
                        break;
                    case SoftwareFactoryEvents.AddTargetEvent:
                        ProcessTargetElement(
                            name: @event.GetValue("Name"),
                            xml: @event.GetValue("Xml"));
                        break;
                    case SoftwareFactoryEvents.AddTaskEvent:
                        ProcessUsingTask(
                            taskName: @event.GetValue("TaskName"),
                            assemblyFile: @event.GetValue("AssemblyFile"));
                        break;
                    case SoftwareFactoryEvents.ChangeProjectItemTypeEvent:
                        ProcessChangeProjectItemType(
                            project: _project,
                            relativeFileName: @event.GetValue("RelativeFileName"),
                            itemType: @event.GetValue("ItemType"));
                        break;
                    case CsProjectEvents.AddCompileDependsOn:
                        ProcessCompileDependsOn(
                            targetName: @event.GetValue("TargetName"));
                        break;
                    case CsProjectEvents.AddImport:
                        ProcessImport(
                            project: @event.GetValue("Project"),
                            condition: @event.GetValue("Condition"));
                        break;
                    case CsProjectEvents.AddBeforeBuild:
                        ProcessBeforeBuild(
                            xml: @event.GetValue("Xml"));
                        break;
                    case CsProjectEvents.AddContentFile:
                        // This and SoftwareFactoryEvents.AddProjectItemEvent can potentially be merged, this one is just
                        // a more explicit version, the other one tends to "work it out".
                        ProcessContent(
                            include: @event.GetValue("Include"),
                            link: @event.GetValue("Link"),
                            copyToOutputDirectory: @event.GetValue("CopyToOutputDirectory"));
                        break;
                    default:
                        Logging.Log.Warning($"VSProject Sync not handling {@event.EventIdentifier}");
                        break;
                }
            }
        }

        private void UpdateFileOnHdd(string filename, string outputContent)
        {
            var se = new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
                                        {
                                            {"FullFileName", filename},
                                            {"Content", outputContent},
                                        });
            _sfEventDispatcher.Publish(se);
        }

        private string LoadProjectFile()
        {
            if (string.IsNullOrWhiteSpace(_projectPath))
                return null;

            var change = _changeManager.FindChange(_projectPath);
            if (change == null)
            {
                _doc = _xmlFileCache.GetFile(_projectPath);
                if (_doc == null)
                {
                    throw new Exception($"Trying to sync project file, but unable to find csproj content. {_projectPath}");
                }
            }
            else
            {
                _doc = XDocument.Parse(change.Content, LoadOptions.PreserveWhitespace);
                _syncProjectFile = (f, c) => change.ChangeContent(c);
            }

            if (_doc.Root == null)
            {
                throw new Exception("_doc.Root is null");
            }

            _namespaces = new XmlNamespaceManager(new NameTable());
            _namespace = _doc.Root.GetDefaultNamespace();
            _namespaces.AddNamespace("ns", _namespace.NamespaceName);

            _projectElement = _doc.XPathSelectElement("/ns:Project", _namespaces);

            return _projectPath;
        }

        private void SyncProjectReferences()
        {
            if (_project.Dependencies().Count <= 0)
            {
                return;
            }

            var itemGroupElement = FindOrCreateProjectReferenceItemGroup();

            foreach (var dependency in _project.Dependencies())
            {
                var projectUrl = string.Format("..\\{0}\\{0}.csproj", dependency.Name);
                var projectReferenceItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:ProjectReference[@Include='{projectUrl}']", _namespaces);
                if (projectReferenceItem != null)
                {
                    continue;
                }

                /*
                <ProjectReference Include="..\Intent.SoftwareFactory\Intent.SoftwareFactory.csproj"/>
                */

                var item = new XElement(XName.Get("ProjectReference", _namespace.NamespaceName));
                item.Add(new XAttribute("Include", projectUrl));

                var projectIdElement = new XElement(XName.Get("Project", _namespace.NamespaceName))
                {
                    Value = $"{{{dependency.Id}}}"
                };
                item.Add(projectIdElement);

                var projectNameElement = new XElement(XName.Get("Name", _namespace.NamespaceName))
                {
                    Value = $"{dependency.Name}"
                };
                item.Add(projectNameElement);

                itemGroupElement.Add(item);
            }
        }

        private void SyncAssemblyReferences()
        {
            if (_project.References().Count == 0)
            {
                return;
            }

            var aReferenceElement = _doc.XPathSelectElement("/ns:Project/ns:ItemGroup/ns:Reference", _namespaces);
            if (aReferenceElement == null)
            {
                throw new Exception("aReferenceElement is null");
            }

            var itemGroupElement = aReferenceElement.Parent;
            if (itemGroupElement == null)
            {
                throw new Exception("itemGroupElement is null");
            }
            foreach (var refrence in _project.References())
            {
                var projectReferenceItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:Reference[@Include='{refrence.Library}']", _namespaces);
                if (projectReferenceItem == null)
                {
                    /*
    <Reference Include="Microsoft.Build">
      <HintPath>..\..\..\..\Program Files (x86)\Reference Assemblies\Microsoft\MSBuild\v14.0\Microsoft.Build.dll</HintPath>
    </Reference>
    <Reference Include="Microsoft.CodeAnalysis, Version=1.3.1.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL">
      <HintPath>..\packages\Microsoft.CodeAnalysis.Common.1.3.2\lib\net45\Microsoft.CodeAnalysis.dll</HintPath>
      <Private>True</Private>
    </Reference>                   
                    */
                    var item = new XElement(XName.Get("Reference", _namespace.NamespaceName));
                    item.Add(new XAttribute("Include", refrence.Library));
                    if (refrence.HasHintPath())
                    {
                        var projectIdElement = new XElement(XName.Get("HintPath", _namespace.NamespaceName))
                        {
                            Value = refrence.HintPath
                        };
                        item.Add(projectIdElement);
                    }

                    itemGroupElement.Add(item);
                }
            }
        }

        private XElement FindOrCreateProjectReferenceItemGroup()
        {
            var aProjectReferenceElement = _doc.XPathSelectElement("/ns:Project/ns:ItemGroup/ns:ProjectReference", _namespaces);

            if (aProjectReferenceElement != null)
            {
                return aProjectReferenceElement.Parent;
            }
            else
            {
                var result = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName));
                _projectElement.Add(result);
                return result;
            }

            //var lastItemGroup = _doc.XPathSelectElements("/ns:Project/ns:ItemGroup", _namespaces).LastOrDefault();
            //if (lastItemGroup == null)
            //{
            //    _projectElement.Add(_codeItems);
            //}
            //else
            //{
            //    if (!lastItemGroup.Elements().Any())
            //    {
            //        result = lastItemGroup;
            //    }
            //    else
            //    {
            //        result = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName));
            //        lastItemGroup.AddAfterSelf(result);
            //    }
            //}

            //return result;
        }

        private XElement FindItemGroupForCodeFiles()
        {
            var codeItems = _doc.XPathSelectElement("/ns:Project/ns:ItemGroup[ns:Compile or ns:Content or ns:None]", _namespaces);

            return codeItems;
        }

        private XElement AddItemGroupForCodeFiles()
        {
            var codeItems = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName));

            var lastItemGroup = _doc.XPathSelectElements("/ns:Project/ns:ItemGroup", _namespaces).LastOrDefault();

            if (lastItemGroup == null)
            {
                _projectElement.Add(codeItems);
            }
            else
            {
                if (!lastItemGroup.Elements().Any())
                {
                    codeItems = lastItemGroup;
                }
                else
                {
                    lastItemGroup.AddAfterSelf(codeItems);
                }
            }

            return codeItems;
        }

        private void ProcessCompileDependsOn(string targetName)
        {
            /*
              <PropertyGroup>
                <CompileDependsOn>$(CompileDependsOn);GulpBuild;</CompileDependsOn>
              </PropertyGroup>
            */

            var element = _doc.XPathSelectElement($"/ns:Project/ns:PropertyGroup/ns:CompileDependsOn[contains(text(),'$(CompileDependsOn);') and contains(text(),'{targetName};')]", _namespaces);
            if (element == null)
            {
                _projectElement.Add(CreateElement(
                    name: "PropertyGroup",
                    subElements: new[]
                    {
                        CreateElement(
                            name: "CompileDependsOn",
                            value: $"$(CompileDependsOn);{targetName};")
                    }));
            }
        }

        private void ProcessImport(string project, string condition)
        {
            project = NormalizePath(project);

            var element = _doc.XPathSelectElement($"/ns:Project/ns:Import[@Project=\"{project}\"]", _namespaces);
            if (element == null)
            {
                _projectElement.Add(element = CreateElement(
                    name: "Import",
                    attributes: new[]
                    {
                        new XAttribute("Project", project),
                        new XAttribute("Condition", condition),
                    }));
            }

            SetOrClearAttribute(attributeName: "Condition", value: condition, xElement: element);
        }

        private void ProcessUsingTask(string taskName, string assemblyFile)
        {
            assemblyFile = NormalizePath(assemblyFile);

            var element = _doc.XPathSelectElement($"/ns:Project/ns:UsingTask[@TaskName=\"{taskName}\"]", _namespaces);
            if (element == null)
            {
                _projectElement.Add(element = CreateElement(
                    name: "UsingTask",
                    attributes: new[]
                    {
                        new XAttribute("TaskName", taskName),
                        new XAttribute("AssemblyFile", assemblyFile),
                    }));
            }

            SetOrClearAttribute(attributeName: "AssemblyFile", value: assemblyFile, xElement: element);
        }

        private XElement GetProjectItem(string fileName)
        {
            var projectItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}']", _namespaces);
            return projectItem;
        }

        private void ProcessRemoveProjectItem(string path)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(_projectPath), path));

            var projectItem = GetProjectItem(relativeFileName);
            projectItem?.Remove();
        }

        private void ProcessAddProjectItem(string path, string itemType, string dependsOn, string copyToOutputDirectory)
        {
            var relativePath = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(_projectPath), path));
            dependsOn = NormalizePath(dependsOn);

            if (string.IsNullOrWhiteSpace(relativePath))
            {
                throw new Exception("relativeFileName is null");
            }

            if (Path.GetExtension(relativePath).Equals(".config", StringComparison.InvariantCultureIgnoreCase))
            {
                copyToOutputDirectory = "PreserveNewest";
            }

            if (itemType == null)
            {
                var fileExtension = !string.IsNullOrEmpty(Path.GetExtension(relativePath)) ? Path.GetExtension(relativePath).Substring(1) : null; //remove the '.'
                switch (fileExtension)
                {
                    case "cs":
                        itemType = "Compile";
                        break;
                    case "csproj":
                        return;
                    default:
                        itemType = "Content";
                        break;
                }
            }

            var metadata = new Dictionary<string, string>();
            if (copyToOutputDirectory != null)
                metadata.Add("CopyToOutputDirectory", copyToOutputDirectory);
            if (dependsOn != null)
                metadata.Add("DependentUpon", dependsOn);

            // GCB - not the most extensible / flexible way of doing this...
            if (Path.GetExtension(relativePath).Equals(".tt", StringComparison.InvariantCultureIgnoreCase))
            {
                metadata.Add("Generator", "TextTemplatingFilePreprocessor");
                metadata.Add("LastGenOutput", Path.GetFileNameWithoutExtension(relativePath) + ".cs");
            }

            var codeItems = FindItemGroupForCodeFiles() ?? AddItemGroupForCodeFiles();

            var projectItem = GetProjectItem(relativePath);
            if (projectItem == null)
            {
                var item = new XElement(XName.Get(itemType, _namespace.NamespaceName));
                item.Add(new XAttribute("Include", relativePath));
                foreach (var data in metadata)
                {
                    var child = new XElement(XName.Get(data.Key, _namespace.NamespaceName))
                    {
                        Value = data.Value
                    };
                    item.Add(child);
                }
                codeItems.Add(item);
            }
            else if (projectItem.Attribute("IntentIgnore")?.Value.ToLower() != "true")
            {
                if (projectItem.Name.LocalName != itemType)
                {
                    projectItem.Name = XName.Get(itemType, projectItem.Name.NamespaceName);
                }
                var children = projectItem.Elements().ToList();
                projectItem.RemoveNodes();
                foreach (var data in metadata)
                {
                    var child = new XElement(XName.Get(data.Key, _namespace.NamespaceName)) { Value = data.Value };
                    projectItem.Add(child);
                }
                foreach (var userAddedMetadata in children.Where(x => metadata.All(y => XName.Get(y.Key, _namespace.NamespaceName) != x.Name)))
                {
                    var child = new XElement(userAddedMetadata.Name) { Value = userAddedMetadata.Value };
                    projectItem.Add(child);
                }
            }
        }

        private void ProcessBeforeBuild(string xml)
        {
            var parsedXml = GetParsedXml(xml);

            var beforeBuildElement = _doc.XPathSelectElement("/ns:Project/ns:Target[@Name=\"BeforeBuild\"]", _namespaces);
            if (beforeBuildElement == null)
            {
                _projectElement.Add(beforeBuildElement = CreateElement(
                    name: "Target",
                    attributes: new[]
                    {
                        new XAttribute("Name", "BeforeBuild"),
                    }));
            }

            if (beforeBuildElement.Elements().All(x => x.ToString() != parsedXml.ToString()))
            {
                beforeBuildElement.Add(parsedXml);
            }
        }

        private void ProcessContent(string include, string link, string copyToOutputDirectory)
        {
            include = NormalizePath(include);
            link = NormalizePath(link);

            var subElements = new List<XElement>();
            if (!string.IsNullOrWhiteSpace(link))
            {
                subElements.Add(CreateElement(name: "Link", value: link));
            }

            if (!string.IsNullOrWhiteSpace(copyToOutputDirectory))
            {
                subElements.Add(CreateElement(name: "CopyToOutputDirectory", value: copyToOutputDirectory));
            }

            var desiredElement = CreateElement(
                name: "Content",
                attributes: new[]
                {
                    new XAttribute("Include", include),
                },
                subElements: subElements);

            var element = GetProjectItem(include);
            if (element == null)
            {
                var codeItems = FindItemGroupForCodeFiles() ?? AddItemGroupForCodeFiles();
                codeItems.Add(desiredElement);
                return;
            }

            ReplaceElementIfNotMatch(element, desiredElement);
        }

        private void ProcessTargetElement(string name, string xml)
        {
            var desiredContent = GetParsedXml(xml);

            var targetElement = _doc.XPathSelectElement($"/ns:Project/ns:Target[@Name=\"{name}\"]", _namespaces);
            if (targetElement == null)
            {
                _projectElement.Add(desiredContent);
                return;
            }

            ReplaceElementIfNotMatch(targetElement, desiredContent);
        }

        private void ProcessChangeProjectItemType(IOutputTarget project, string relativeFileName, string itemType)
        {
            relativeFileName = NormalizePath(relativeFileName);

            var projectItem = GetProjectItem(relativeFileName);
            if (projectItem == null)
            {
                //WTF
                throw new Exception($"Cant from config file {relativeFileName} in project file {_projectPath}");
            }

            //Make sure it's None not Content
            if (projectItem.Name.LocalName != itemType)
            {
                projectItem.Name = XName.Get(itemType, projectItem.Name.NamespaceName);
            }

        }

        private XElement CreateElement(string name, string value = null, IEnumerable<XAttribute> attributes = null, IEnumerable<XElement> subElements = null)
        {
            attributes = attributes ?? new XAttribute[0];
            subElements = subElements ?? new XElement[0];

            var newElement = new XElement(XName.Get(name, _namespace.NamespaceName));
            if (value != null)
            {
                newElement.Value = value;
            }

            foreach (var attribute in attributes)
            {
                newElement.Add(attribute);
            }

            foreach (var element in subElements)
            {
                newElement.Add(element);
            }

            return newElement;
        }

        private XElement GetParsedXml(string xml)
        {
            var parsedXml = XElement.Parse(xml);
            foreach (var xElement in parsedXml.DescendantsAndSelf())
            {
                xElement.Name = _namespace + xElement.Name.LocalName;
            }

            return parsedXml;
        }

        private static void ReplaceElementIfNotMatch(XNode current, XElement desired)
        {
            if (desired.ToString() == current.ToString())
            {
                return;
            }

            var parentElement = current.Parent;
            if (parentElement == null)
            {
                throw new Exception("parentElement is null");
            }

            var previousElement = current.PreviousNode;
            current.Remove();

            if (previousElement != null)
            {
                previousElement.AddAfterSelf(desired);
            }
            else
            {
                parentElement.AddFirst(desired);
            }

        }

        private static void SetOrClearAttribute(string attributeName, string value, XElement xElement)
        {
            var attribute = xElement.Attributes().SingleOrDefault(x => x.Name == attributeName);
            if (value == null)
            {
                attribute?.Remove();
            }

            if (value != null && attribute == null)
            {
                xElement.Add(attribute = new XAttribute(attributeName, value));
            }

            if (value != null && attribute.Value != value)
            {
                attribute.Value = value;
            }
        }

        private static string NormalizePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            // .csproj and solution files use backslashes even on Mac
            value = value.Replace("/", @"\");

            // Replace double occurrences of folder seperators with single seperator. IE, turn a path like Dev\\Folder to Dev\Folder
            while (value.Contains(@"\\"))
                value = value.Replace(@"\\", @"\");

            return value;
        }

    }
}

