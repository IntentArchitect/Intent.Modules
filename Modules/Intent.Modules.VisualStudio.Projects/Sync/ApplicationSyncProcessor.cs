using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    [Description("Visual Studio Project File Syncer")]
    public class ApplicationSyncProcessor : FactoryExtensionBase, IExecutionLifeCycle
    {
        private readonly ISoftwareFactoryEventDispatcher _eventDispatcher;
        private readonly IChanges _changeManager;
        private readonly IXmlFileCache _fileCache;
        private readonly Dictionary<string, List<SoftwareFactoryEvent>> _actions;

        public override string Id => "Intent.ApplicationSyncProcessor";

        public ApplicationSyncProcessor(ISoftwareFactoryEventDispatcher eventDispatcher, IXmlFileCache fileCache, IChanges changeManager)
        {
            Order = 90;
            _changeManager = changeManager;
            _fileCache = fileCache;
            _actions = new Dictionary<string, List<SoftwareFactoryEvent>>();
            _eventDispatcher = eventDispatcher;
            //Subscribe to all the project change events
            _eventDispatcher.Subscribe(SoftwareFactoryEvents.AddProjectItemEvent, Handle);
            _eventDispatcher.Subscribe(SoftwareFactoryEvents.RemoveProjectItemEvent, Handle);
            _eventDispatcher.Subscribe(SoftwareFactoryEvents.AddTargetEvent, Handle);
            _eventDispatcher.Subscribe(SoftwareFactoryEvents.AddTaskEvent, Handle);
            _eventDispatcher.Subscribe(SoftwareFactoryEvents.ChangeProjectItemTypeEvent, Handle);

            _eventDispatcher.Subscribe(CsProjectEvents.AddImport, Handle);
            _eventDispatcher.Subscribe(CsProjectEvents.AddCompileDependsOn, Handle);
            _eventDispatcher.Subscribe(CsProjectEvents.AddBeforeBuild, Handle);
            _eventDispatcher.Subscribe(CsProjectEvents.AddContentFile, Handle);
        }

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                SyncProjectFiles(application);
            }
        }

        public void SyncProjectFiles(IApplication application)
        {
            foreach (var project in _actions)
            {
                var vsproject = application.Projects.FirstOrDefault(x => x.Id == new Guid(project.Key));
                if (vsproject == null)
                {
                    //This scenario occurs when projects have been deleted
                    continue;
                }

                switch (vsproject.ProjectType.Id)
                {
                    case VisualStudioProjectTypeIds.CSharpLibrary:
                    case VisualStudioProjectTypeIds.ConsoleAppNetFramework:
                    case VisualStudioProjectTypeIds.NodeJsConsoleApplication:
                    case VisualStudioProjectTypeIds.WcfApplication:
                    case VisualStudioProjectTypeIds.WebApiApplication:
                        new FrameworkProjectSyncProcessor(_eventDispatcher, _fileCache, _changeManager, vsproject).Process(project.Value);
                        break;
                    case VisualStudioProjectTypeIds.CoreCSharpLibrary:
                    case VisualStudioProjectTypeIds.CoreWebApp:
                        new CoreProjectSyncProcessor(_eventDispatcher, _fileCache, _changeManager, vsproject).Process(project.Value);
                        break;
                    default:
                        throw new Exception($"No syncer configured for project type '{vsproject.ProjectType.Name}' ({vsproject.ProjectType.Id})");
                }
            }

            _actions.Clear();
        }

        public void Handle(SoftwareFactoryEvent @event)
        {
            var projectId = @event.GetValue("ProjectId");
            if (!_actions.ContainsKey(projectId))
            {
                _actions[projectId] = new List<SoftwareFactoryEvent>();
            }
            _actions[projectId].Add(@event);
        }
    }
}
