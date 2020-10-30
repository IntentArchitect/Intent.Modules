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
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.FileAdded, Handle);
            _sfEventDispatcher.Subscribe(SoftwareFactoryEvents.FileRemoved, Handle);
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
            foreach (var outputEvent in _actions)
            {
                var vsProject = application.OutputTargets.FirstOrDefault(x => x.Id == outputEvent.Key)?.GetTargetPath()[0];
                if (vsProject == null)
                {
                    //This scenario occurs when projects have been deleted
                    continue;
                }

                switch (vsProject.Type)
                {
                    case VisualStudioProjectTypeIds.CSharpLibrary:
                    case VisualStudioProjectTypeIds.ConsoleAppNetFramework:
                    case VisualStudioProjectTypeIds.NodeJsConsoleApplication:
                    case VisualStudioProjectTypeIds.WcfApplication:
                    case VisualStudioProjectTypeIds.WebApiApplication:
                        new FrameworkProjectSyncProcessor(_projectRegistry[vsProject.Id].FilePath, _sfEventDispatcher, _fileCache, _changeManager, vsProject).Process(outputEvent.Value);
                        break;
                    case VisualStudioProjectTypeIds.CoreCSharpLibrary:
                    case VisualStudioProjectTypeIds.CoreWebApp:
                        new CoreProjectSyncProcessor(_projectRegistry[vsProject.Id].FilePath, _sfEventDispatcher, _fileCache, _changeManager, vsProject).Process(outputEvent.Value);
                        break;
                    default:
                        throw new Exception($"No syncer configured for project '{vsProject.Name}' with type ({vsProject.Type})");
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
            var outputTargetId = @event.GetValue("OutputTargetId");
            if (!_actions.ContainsKey(outputTargetId))
            {
                _actions[outputTargetId] = new List<SoftwareFactoryEvent>();
            }
            _actions[outputTargetId].Add(@event);
        }
    }
}
