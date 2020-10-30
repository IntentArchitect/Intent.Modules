using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Constants;
using Intent.Modules.VisualStudio.Projects.Events;
using Intent.SoftwareFactory;
using Intent.Engine;
using Intent.Eventing;
using Intent.Modules.VisualStudio.Projects.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.VisualStudio.Projects.Sync
{
    [Description("Visual Studio Project File Syncer")]
    public class ApplicationSyncProcessor : FactoryExtensionBase, IExecutionLifeCycle
    {
        private readonly ISoftwareFactoryEventDispatcher _sfEventDispatcher;
        private readonly IChanges _changeManager;
        private readonly IXmlFileCache _fileCache;
        private readonly Dictionary<string, List<SoftwareFactoryEvent>> _actions;
        private readonly IDictionary<string, IVisualStudioProjectTemplate> _projectRegistry = new Dictionary<string, IVisualStudioProjectTemplate>();

        public override string Id => "Intent.ApplicationSyncProcessor";

        public ApplicationSyncProcessor(ISoftwareFactoryEventDispatcher sfEventDispatcher, IXmlFileCache fileCache, IChanges changeManager)
        {
            Order = 90;
            _changeManager = changeManager;
            _fileCache = fileCache;
            _actions = new Dictionary<string, List<SoftwareFactoryEvent>>();
            _sfEventDispatcher = sfEventDispatcher;
            //Subscribe to all the project change events
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.AddProjectItemEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.RemoveProjectItemEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.AddTargetEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.AddTaskEvent, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.ChangeProjectItemTypeEvent, Handle);

            _sfEventDispatcher.Subscribe(CsProjectEvents.AddImport, Handle);
            _sfEventDispatcher.Subscribe(CsProjectEvents.AddCompileDependsOn, Handle);
            _sfEventDispatcher.Subscribe(CsProjectEvents.AddBeforeBuild, Handle);
            _sfEventDispatcher.Subscribe(CsProjectEvents.AddContentFile, Handle);

        }

        public void OnStep(IApplication application, string step)
        {
            if (step == ExecutionLifeCycleSteps.BeforeTemplateRegistrations)
            {
                application.EventDispatcher.Subscribe<VisualStudioProjectCreatedEvent>(HandleEvent);
            }
            if (step == ExecutionLifeCycleSteps.AfterTemplateExecution)
            {
                SyncProjectFiles(application);
            }
        }

        public void SyncProjectFiles(IApplication application)
        {
            foreach (var project in _actions)
            {
                var vsproject = application.Projects.FirstOrDefault(x => x.Id == project.Key)?.GetTargetPath()[0];
                if (vsproject == null)
                {
                    //This scenario occurs when projects have been deleted
                    continue;
                }

                switch (vsproject.Type)
                {
                    case VisualStudioProjectTypeIds.CSharpLibrary:
                    case VisualStudioProjectTypeIds.ConsoleAppNetFramework:
                    case VisualStudioProjectTypeIds.NodeJsConsoleApplication:
                    case VisualStudioProjectTypeIds.WcfApplication:
                    case VisualStudioProjectTypeIds.WebApiApplication:
                        new FrameworkProjectSyncProcessor(_projectRegistry[vsproject.Id].FilePath, _sfEventDispatcher, _fileCache, _changeManager, vsproject).Process(project.Value);
                        break;
                    case VisualStudioProjectTypeIds.CoreCSharpLibrary:
                    case VisualStudioProjectTypeIds.CoreWebApp:
                        new CoreProjectSyncProcessor(_projectRegistry[vsproject.Id].FilePath, _sfEventDispatcher, _fileCache, _changeManager, vsproject).Process(project.Value);
                        break;
                    default:
                        throw new Exception($"No syncer configured for project '{vsproject.Name}' with type ({vsproject.Type})");
                }
            }

            _actions.Clear();
        }

        private void HandleEvent(VisualStudioProjectCreatedEvent @event)
        {
            if (_projectRegistry.ContainsKey(@event.ProjectId))
            {
                throw new Exception($"Attempted to add project with same project Id [{@event.ProjectId}] (location: {@event.TemplateInstance.FilePath})");
            }
            _projectRegistry.Add(@event.ProjectId, @event.TemplateInstance);
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
