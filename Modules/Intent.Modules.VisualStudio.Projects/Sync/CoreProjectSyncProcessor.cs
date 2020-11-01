using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    public class CoreProjectSyncProcessor
    {
        private readonly IXmlFileCache _xmlFileCache;
        private readonly IChanges _changeManager;
        private readonly string _projectPath;
        private readonly ISoftwareFactoryEventDispatcher _softwareFactoryEventDispatcher;
        private readonly IOutputTarget _project;

        private Action<string, string> _syncProjectFile;
        private XDocument _doc;
        private XmlNamespaceManager _namespaces;
        private XNamespace _namespace;
        private XElement _projectElement;

        public CoreProjectSyncProcessor(
            string projectPath,
            ISoftwareFactoryEventDispatcher softwareFactoryEventDispatcher,
            IXmlFileCache xmlFileCache,
            IChanges changeManager,
            IOutputTarget project)
        {
            _projectPath = projectPath;
            _softwareFactoryEventDispatcher = softwareFactoryEventDispatcher;
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

            _project.SyncProjectReferences(_doc);

            //run events
            ProcessEvents(events);

            var currentProjectFileContent = "";
            if (File.Exists(filename))
            {
                var currentProjectFile = XDocument.Parse(File.ReadAllText(filename), LoadOptions.PreserveWhitespace);
                currentProjectFileContent = currentProjectFile.ToString();
            }
            var outputContent = _doc.ToString();
            //trying to do a schemantic comparision as VS does inconsistence formatting 
            if (currentProjectFileContent != outputContent)
            {
                Logging.Log.Debug($"Syncing changes to Project File {filename}");
                //string content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + outputContent;
                _syncProjectFile(filename, outputContent);
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
                            copyToOutputDirectory: @event.TryGetValue("CopyToOutputDirectory"),
                            linkSource: null);
                        break;
                    case CsProjectEvents.AddContentFile:
                        ProcessAddProjectItem(
                            path: Path.Combine(Path.GetDirectoryName(_projectPath), @event.GetValue("Link")),
                            itemType: "Content",
                            dependsOn: null,
                            copyToOutputDirectory: @event.TryGetValue("CopyToOutputDirectory"),
                            linkSource: @event.GetValue("Include"));
                        break;
                    case SoftwareFactoryEvents.FileRemoved:
                        ProcessRemoveProjectItem(
                            path: @event.GetValue("Path"));
                        break;
                    case CsProjectEvents.AddCompileDependsOn:
                        ProcessCompileDependsOn(
                            targetName: @event.GetValue("TargetName"));
                        break;
                    case CsProjectEvents.AddBeforeBuild:
                    case SoftwareFactoryEvents.AddTargetEvent:
                    case SoftwareFactoryEvents.AddTaskEvent:
                    case SoftwareFactoryEvents.ChangeProjectItemTypeEvent:
                    case CsProjectEvents.AddImport:
                        // NOP
                        break;
                    default:
                        Logging.Log.Warning($"Core VSProject Sync not handling {@event.EventIdentifier}");
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
            _softwareFactoryEventDispatcher.Publish(se);
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

        private XElement GetOrCreateItemGroupFor(string type)
        {
            var itemGroupElement = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup[ns:{type}]", _namespaces);
            if (itemGroupElement == null)
            {
                if (_doc.Root == null) throw new Exception("_doc.Root is null.");
                _doc.Root.Add(
                    "  ",
                    itemGroupElement = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName)),
                    Environment.NewLine,
                    Environment.NewLine);
            }

            return itemGroupElement;
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

        private XElement GetFileItem(string fileName)
        {
            var projectItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}' or @Update='{fileName}' or @Link='{fileName}']", _namespaces);

            return projectItem;
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

        private void ProcessAddProjectItem(string path, string itemType, string dependsOn, string copyToOutputDirectory, string linkSource)
        {
            var relativeFileName = NormalizePath(Path.GetRelativePath(Path.GetDirectoryName(_projectPath), path));
            dependsOn = NormalizePath(dependsOn);
            linkSource = NormalizePath(linkSource);

            if (string.IsNullOrWhiteSpace(itemType))
                itemType = null;

            if (string.IsNullOrWhiteSpace(copyToOutputDirectory))
                copyToOutputDirectory = null;

            if (string.IsNullOrWhiteSpace(dependsOn))
                dependsOn = null;

            if (string.IsNullOrWhiteSpace(relativeFileName))
                throw new Exception("relativeFileName is null");

            var extension = !string.IsNullOrEmpty(Path.GetExtension(relativeFileName))
                ? Path.GetExtension(relativeFileName).Substring(1) //remove the '.'
                : null;

            var metadata = new Dictionary<string, string>();

            if (copyToOutputDirectory != null)
            {
                metadata.Add("CopyToOutputDirectory", copyToOutputDirectory);
            }

            if (dependsOn != null)
            {
                metadata.Add("DesignTime", "True");
                metadata.Add("AutoGen", "True");
                metadata.Add("DependentUpon", dependsOn);
            }

            // Setup defaults, not the best kind of extensibility unfortunately.
            bool implicitlyPresent;
            switch (extension)
            {
                case "cs":
                    itemType = itemType ?? "Compile";

                    implicitlyPresent =
                        linkSource == null &&
                        itemType == "Compile" &&
                        !metadata.Any();
                    break;
                case "tt":
                    itemType = itemType ?? "None";
                    metadata.Add("Generator", "TextTemplatingFilePreprocessor");
                    metadata.Add("LastGenOutput", Path.GetFileNameWithoutExtension(relativeFileName) + ".cs");

                    implicitlyPresent = false;
                    break;
                case "config":
                    itemType = itemType ?? "Content";
                    //metadata["CopyToOutputDirectory"] = "PreserveNewest"; // Why is this needed? I've taken out.

                    implicitlyPresent =
                        linkSource == null &&
                        itemType == "Content" &&
                        !metadata.Any();
                    break;
                default:
                    itemType = itemType ?? "Content";

                    implicitlyPresent =
                        linkSource == null &&
                        itemType == "Content" &&
                        !metadata.Any();
                    break;
            }

            var itemElement = GetFileItem(linkSource ?? relativeFileName);
            if (itemElement?.Attribute("IntentIgnore")?.Value.ToLower() != "true")
            {
                if (implicitlyPresent)
                {
                    itemElement?.Remove();
                    return;
                }

                if (itemElement == null)
                {
                    var targetElement = GetOrCreateItemGroupFor(itemType);
                    targetElement.Add(
                        Environment.NewLine,
                        "    ",
                        itemElement = new XElement(XName.Get(itemType, _namespace.NamespaceName)),
                        Environment.NewLine,
                        "  ");
                }

                if (itemElement.Name.LocalName != itemType)
                {
                    itemElement.Name = XName.Get(itemType, itemElement.Name.NamespaceName);
                }

                itemElement.SetAttributeValue(XName.Get("Update", _namespace.NamespaceName), linkSource == null ? relativeFileName : null);
                itemElement.SetAttributeValue(XName.Get("Link", _namespace.NamespaceName), linkSource != null ? relativeFileName : null);
                itemElement.SetAttributeValue(XName.Get("Include", _namespace.NamespaceName), linkSource);

                var subelementWasAdded = false;
                foreach (var item in metadata)
                {
                    var subElement = itemElement.Elements().SingleOrDefault(x => x.Name == XName.Get(item.Key, _namespace.NamespaceName));
                    if (subElement == null)
                    {
                        subelementWasAdded = true;
                        itemElement.Add(
                            Environment.NewLine,
                            "      ",
                            subElement = new XElement(XName.Get(item.Key, _namespace.NamespaceName)));
                    }

                    subElement.Value = item.Value;
                }

                if (subelementWasAdded)
                {
                    itemElement.Add(
                        Environment.NewLine,
                        "    ");
                }
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

