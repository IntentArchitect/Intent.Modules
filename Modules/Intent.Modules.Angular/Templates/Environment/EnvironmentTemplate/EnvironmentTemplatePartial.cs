using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Environment.EnvironmentTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class EnvironmentTemplate : IntentTypescriptProjectItemTemplateBase<object>
    {
        private IList<ConfigVariable> _configVariables = new List<ConfigVariable>();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Environment.EnvironmentTemplate";

        public EnvironmentTemplate(IProject project, object model) : base(TemplateId, project, model)
        {
            project.Application.EventDispatcher.Subscribe(AngularConfigVariableRequiredEvent.EventId, HandleConfigVariableRequiredEvent);
        }

        private void HandleConfigVariableRequiredEvent(ApplicationEvent @event)
        {
            _configVariables.Add(new ConfigVariable(
                name: @event.GetValue(AngularConfigVariableRequiredEvent.VariableId),
                defaultValue: @event.GetValue(AngularConfigVariableRequiredEvent.DefaultValueId)));
        }

        public override string RunTemplate()
        {
            var meta = GetMetadata();
            var fullFileName = Path.Combine(meta.GetFullLocationPath(), meta.FileNameWithExtension());

            var source = LoadOrCreate(fullFileName);
            var file = new TypescriptFile(source);
            var variable = file.VariableDeclarations().First();

            foreach (var configVariable in _configVariables)
            {
                if (!variable.PropertyAssignmentExists(configVariable.Name))
                {
                    variable.AddPropertyAssignment($@",
  {configVariable.Name}: {configVariable.DefaultValue}");
                }
            }

            return file.GetChangedSource();
        }

        private string LoadOrCreate(string fullFileName)
        {
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : base.RunTemplate();
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "environment",
                fileExtension: "ts", // Change to desired file extension.
                defaultLocationInProject: "Client/src/environments"
            );
        }


    }

    internal class ConfigVariable
    {
        public ConfigVariable(string name, string defaultValue)
        {
            Name = name;
            DefaultValue = defaultValue;
        }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
    }
}