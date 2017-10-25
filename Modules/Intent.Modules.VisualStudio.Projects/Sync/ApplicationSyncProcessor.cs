using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.SoftwareFactory;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Plugins;
using Intent.SoftwareFactory.Plugins.FactoryExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    [Description("Visual Studio Project File Syncer")]
    public class ApplicationSyncProcessor : FactoryExtensionBase, IExecutionLifeCycle
    {
        private readonly ISoftwareFactoryEventDispatcher _eventDispatcher;
        private readonly IChanges _changeManager;
        private readonly IXmlFileCache _fileCache;
        private readonly Dictionary<string, List<SoftwareFactoryEvent>> _actions;

        public override string Id
        {
            get
            {
                return "Intent.ApplicationSyncProcessor";
            }
        }

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
                var vsproject = application.Projects.Where(x => x.Id == new Guid(project.Key)).FirstOrDefault();
                //This scenario occurs when projects have been deleted
                if (vsproject != null)
                {
                    ProjectSyncProcessor p = new ProjectSyncProcessor(_eventDispatcher, _fileCache, _changeManager, vsproject);
                    p.Process(project.Value);
                }
            }

            _actions.Clear();
        }

        public void Handle(SoftwareFactoryEvent @event)
        {
            string projectId = @event.GetValue("ProjectId");
            if (!_actions.ContainsKey(projectId))
            {
                _actions[projectId] = new List<SoftwareFactoryEvent>();
            }
            _actions[projectId].Add(@event);
        }
    }
}
