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
using Intent.Modules.Common.TypeScript.Editor;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Typescript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Environment.EnvironmentTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class EnvironmentTemplate : TypeScriptTemplateBase<object>
    {
        private IList<ConfigVariable> _configVariables = new List<ConfigVariable>();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Templates.Environment.EnvironmentTemplate";

        public EnvironmentTemplate(IProject project, object model) : base(TemplateId, project, model, TypescriptTemplateMode.UpdateFile)
        {
            project.Application.EventDispatcher.Subscribe(AngularConfigVariableRequiredEvent.EventId, HandleConfigVariableRequiredEvent);
        }

        private void HandleConfigVariableRequiredEvent(ApplicationEvent @event)
        {
            _configVariables.Add(new ConfigVariable(
                name: @event.GetValue(AngularConfigVariableRequiredEvent.VariableId),
                defaultValue: @event.GetValue(AngularConfigVariableRequiredEvent.DefaultValueId)));
        }

        protected override void ApplyFileChanges(TypeScriptFile file)
        {
            var variable = file.VariableDeclarations().First();

            foreach (var configVariable in _configVariables)
            {
                var assigned = variable.GetAssignedValue<TypeScriptObjectLiteralExpression>();
                if (assigned != null && !assigned.PropertyAssignmentExists(configVariable.Name))
                {
                    assigned.AddPropertyAssignment($@",
  {configVariable.Name}: {configVariable.DefaultValue}");
                }
            }
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