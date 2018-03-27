using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Sync.Events;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.VisualStudio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    public class ProjectSyncProcessor
    {
        private readonly IXmlFileCache _xmlFileCache;
        private readonly IChanges _changeManager;
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        public IProject Project { get; }
        private Action<string, string> _syncProjectFile;

        private XDocument _doc;
        private XmlNamespaceManager _namespaces;
        private XNamespace _namespace;
        private XElement _projectElement;
        private XElement _codeItems;

        public ProjectSyncProcessor(ISoftwareFactoryEventDispatcher eventDispatcher,
            IXmlFileCache xmlFileCache,
            IChanges changeManager,
            IProject project)
        {
            _changeManager = changeManager;
            _xmlFileCache = xmlFileCache;
            _sfEventDispatcher = eventDispatcher;
            Project = project;
            _syncProjectFile = UpdateFileOnHDD;
        }

        public void Process(List<SoftwareFactoryEvent> events)
        {
            string filename = LoadProjectFile();

            if (string.IsNullOrWhiteSpace(filename))
                return;

            filename = Path.GetFullPath(filename);

            //run events
            ProcessEvents(events);
            SyncAssemblyReferences();
            SyncProjectReferences();

            string currentProjectFileContent = "";
            if (File.Exists(filename))
            {
                var currentProjectFile = XDocument.Parse(File.ReadAllText(filename));
                currentProjectFileContent = currentProjectFile.ToStringUTF8();
            }
            string outputContent = _doc.ToStringUTF8();
            //trying to do a schemantic comparision as VS does inconsistence formatting 
            if (currentProjectFileContent != outputContent)
            {
                Logging.Log.Debug($"Syncing changes to Project File {filename}");
                //string content = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + Environment.NewLine + outputContent;
                _syncProjectFile(filename, outputContent);
            }
        }

        private void UpdateFileOnHDD(string filename, string outputContent)
        {
            var se = new SoftwareFactoryEvent(SoftwareFactoryEvents.OverwriteFileCommand, new Dictionary<string, string>
                                        {
                                            {"FullFileName", filename},
                                            {"Content", outputContent},
                                        });
            _sfEventDispatcher.Publish(se);
        }

        private void CompareToExpectedResult(string expected, string output)
        {
            if (output != expected)
            {
                StringBuilder errorMessage = new StringBuilder();

                if (output.Length != expected.Length)
                {
                    if (output.Length < expected.Length)
                    {
                        errorMessage.AppendLine(" (Output shorter)");
                    }
                    else
                    {
                        errorMessage.AppendLine(" (Expected shorter)");
                    }
                }

                for (int i = 0; i < Math.Min(output.Length, expected.Length); i++)
                {
                    if (output[i] != expected[i])
                    {
                        errorMessage.AppendLine("first difference at position " + i);
                        var from = Math.Max(0, i - 500);
                        errorMessage.AppendLine("output : " + output.Substring(from, Math.Min(1000, output.Length - from)));
                        errorMessage.AppendLine("expect : " + expected.Substring(from, Math.Min(1000, expected.Length - from)));
                        break;
                    }
                }
                throw new Exception(errorMessage.ToString());
            }
        }

        private void SyncProjectReferences()
        {
            if (Project.Dependencies().Count > 0)
            {

                var itemGroupElement = FindProjectReferenceItemGroup();

                foreach (var dependency in Project.Dependencies())
                {
                    string projectUrl = string.Format("..\\{0}\\{0}.csproj", dependency.Name);
                    var projectReferenceItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:ProjectReference[@Include='{projectUrl}']", _namespaces);
                    if (projectReferenceItem == null)
                    {
                        /*
        <ProjectReference Include="..\Intent.SoftwareFactory\Intent.SoftwareFactory.csproj">
          <Project>{c5a8f278-d3a4-4f93-98d1-964147f54d6e}</Project>
          <Name>Intent.SoftwareFactory</Name>
        </ProjectReference>                     */

                        var item = new XElement(XName.Get("ProjectReference", _namespace.NamespaceName));
                        item.Add(new XAttribute("Include", projectUrl));

                        var projectIdElement = new XElement(XName.Get("Project", _namespace.NamespaceName));
                        projectIdElement.Value = $"{{{dependency.Id}}}";
                        item.Add(projectIdElement);

                        var projectNameElement = new XElement(XName.Get("Name", _namespace.NamespaceName));
                        projectNameElement.Value = $"{dependency.Name}";
                        item.Add(projectNameElement);

                        itemGroupElement.Add(item);
                    }
                }
            }
        }

        private void SyncAssemblyReferences()
        {
            var aReferenceElement = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:Reference", _namespaces);
            if (aReferenceElement == null && Project.References().Count == 0)
            {
                return;
            }

            var itemGroupElement = aReferenceElement.Parent;

            foreach (var refrence in Project.References())
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
                        var projectIdElement = new XElement(XName.Get("HintPath", _namespace.NamespaceName));
                        projectIdElement.Value = refrence.HintPath;
                        item.Add(projectIdElement);
                    }

                    itemGroupElement.Add(item);
                }
            }
        }

        private string LoadProjectFile()
        {
            string filename = Project.ProjectFile();
            if (string.IsNullOrWhiteSpace(filename))
                return null;
            _doc = _xmlFileCache.GetFile(filename);

            if (_doc == null)
            {
                var change = _changeManager.FindChange(filename);
                if (change == null)
                {
                    throw new Exception($"Trying to sync project file, but unable to find csproj content. {filename}");
                }
                _doc = XDocument.Parse(change.Content);

                _syncProjectFile = (f, c) => change.ChangeContent(c);
            }

            _namespaces = new XmlNamespaceManager(new NameTable());
            _namespace = _doc.Root.GetDefaultNamespace();
            _namespaces.AddNamespace("ns", _namespace.NamespaceName);

            _projectElement = _doc.XPathSelectElement("/ns:Project", _namespaces);

            FindItemGroupForCodeFiles();

            return filename;
        }

        private XElement FindProjectReferenceItemGroup()
        {
            XElement result = null;
            var aProjectReferenceElement = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/ns:ProjectReference", _namespaces);

            if (aProjectReferenceElement != null)
            {
                return aProjectReferenceElement.Parent;
            }


            var lastItemGroup = _doc.XPathSelectElements("/ns:Project/ns:ItemGroup", _namespaces).LastOrDefault();
            if (lastItemGroup == null)
            {
                _projectElement.Add(_codeItems);
            }
            else
            {
                if (lastItemGroup.Elements().Count() == 0)
                {
                    result = lastItemGroup;
                }
                else
                {
                    result = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName));
                    lastItemGroup.AddAfterSelf(result);
                }
            }

            return result;
        }

        private void FindItemGroupForCodeFiles()
        {
            _codeItems = _doc.XPathSelectElement("/ns:Project/ns:ItemGroup[ns:Compile or ns:Content or ns:None]", _namespaces);

            if (_codeItems == null)
            {

                var lastItemGroup = _doc.XPathSelectElements("/ns:Project/ns:ItemGroup", _namespaces).LastOrDefault();
                if (lastItemGroup == null)
                {
                    _projectElement.Add(_codeItems);
                }
                else
                {
                    if (lastItemGroup.Elements().Count() == 0)
                    {
                        _codeItems = lastItemGroup;
                    }
                    else
                    {
                        _codeItems = new XElement(XName.Get("ItemGroup", _namespace.NamespaceName));
                        lastItemGroup.AddAfterSelf(_codeItems);
                    }
                }
            }
        }

        public void ProcessEvents(List<SoftwareFactoryEvent> events)
        {
            foreach (var @event in events)
            {
                switch (@event.EventIdentifier)
                {
                    case SoftwareFactoryEvents.AddProjectItemEvent:
                        var x = new AddProjectItemEvent(Project, @event.GetValue("RelativeFileName"), @event.TryGetValue("ItemType"));

                        if (@event.TryGetValue("Depends On") != null)
                        {
                            x.DependentUpon(@event.TryGetValue("Depends On"));
                        }

                        if (@event.TryGetValue("IntentGenType") != null)
                        {
                            x.Generated(@event.TryGetValue("IntentGenType"));
                        }

                        if (@event.TryGetValue("CopyToOutputDirectory") != null)
                        {
                            x.CopyToOutputDirectory(@event.TryGetValue("CopyToOutputDirectory"));
                        }

                        Process(x);
                        break;
                    case SoftwareFactoryEvents.RemoveProjectItemEvent:
                        var r = new RemoveProjectItemEvent(Project, @event.GetValue("RelativeFileName"));
                        Process(r);
                        break;

                    case SoftwareFactoryEvents.AddTargetEvent:
                        Process(new AddTargetEvent(Project, @event.GetValue("Name"), @event.GetValue("Condition"), @event.GetValue("Xml")));
                        break;
                    case SoftwareFactoryEvents.AddTaskEvent:
                        Process(new AddTaskEvent(Project, @event.GetValue("TaskName"), @event.GetValue("AssemblyFile")));
                        break;
                    case SoftwareFactoryEvents.ChangeProjectItemTypeEvent:
                        Process(new ChangeProjectItemTypeEvent(Project, @event.GetValue("RelativeFileName"), @event.GetValue("ItemType")));
                        break;
                    default:
                        Logging.Log.Warning($"VSProject Sync not handling {@event.EventIdentifier}");
                        break;
                }
            }
        }

        private XElement GetProjectItem(string fileName)
        {
            var projectItem = _doc.XPathSelectElement($"/ns:Project/ns:ItemGroup/*[@Include='{fileName}']", _namespaces);
            return projectItem;
        }

        private void Process (RemoveProjectItemEvent @event)
        {
            var projectItem = GetProjectItem(@event.RelativeFileName);
            if (projectItem != null)
            {
                projectItem.Remove();
            }
        }

        private void Process(AddProjectItemEvent @event)
        {

            var projectItem = GetProjectItem(@event.RelativeFileName);
            if (projectItem == null)
            {
                var item = new XElement(XName.Get(@event.ItemType, _namespace.NamespaceName));
                item.Add(new XAttribute("Include", @event.RelativeFileName));
                foreach (var metaData in @event.MetaData)
                {
                    var child = new XElement(XName.Get(metaData.Key, _namespace.NamespaceName));
                    child.Value = metaData.Value;
                    item.Add(child);
                }
                _codeItems.Add(item);
            }
            else
            {
                if (projectItem.Name.LocalName != @event.ItemType)
                {
                    projectItem.Name = XName.Get(@event.ItemType, projectItem.Name.NamespaceName);
                }
                var children = projectItem.Elements().ToList();
                projectItem.RemoveNodes();
                foreach (var metaData in @event.MetaData)
                {
                    var child = new XElement(XName.Get(metaData.Key, _namespace.NamespaceName));
                    child.Value = metaData.Value;
                    projectItem.Add(child);
                }
                foreach (var userAddedMetaData in children.Where(x => @event.MetaData.All(y => XName.Get(y.Key, _namespace.NamespaceName) != x.Name)))
                {
                    var child = new XElement(userAddedMetaData.Name);
                    child.Value = userAddedMetaData.Value;
                    projectItem.Add(child);
                }
            }
        }

        private void Process(AddTargetEvent @event)
        {
            string xpath = $"/ns:Project/ns:Target[@Name=\"{@event.Name}\" and @Condition=\"{@event.Condition}\"]";
            var transElement = _doc.XPathSelectElement(xpath, _namespaces);
            if (transElement == null)
            {
                var newNode = XElement.Parse(@event.Xml);
                foreach (XElement ce in newNode.DescendantsAndSelf())
                {
                    ce.Name = _namespace + ce.Name.LocalName;
                }
                _projectElement.Add(newNode);
            }
        }

        private void Process(AddTaskEvent @event)
        {
            var element = _doc.XPathSelectElement($"/ns:Project/ns:UsingTask[@TaskName='{@event.TaskName}']", _namespaces);
            if (element == null)
            {
                var transformTaskRegister = new XElement(XName.Get("UsingTask", _namespace.NamespaceName));
                transformTaskRegister.Add(new XAttribute("TaskName", @event.TaskName));
                transformTaskRegister.Add(new XAttribute("AssemblyFile", @event.AssemblyFile));
                _projectElement.Add(transformTaskRegister);
            }

        }

        private void Process(ChangeProjectItemTypeEvent @event)
        {
            var projectItem = GetProjectItem(@event.RelativeFileName);
            if (projectItem == null)
            {
                //WTF
                throw new Exception($"Cant from config file {@event.RelativeFileName} in project file {@event.Project.ProjectFile()}");
            }

            //Make sure it's None not Content
            if (projectItem.Name.LocalName != @event.ItemType)
            {
                projectItem.Name = XName.Get(@event.ItemType, projectItem.Name.NamespaceName);
            }

        }

    }
}

