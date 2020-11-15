using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Eventing;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Modules.Angular.Api;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.TypeScript.Templates.TypescriptTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Templates.Environment.EnvironmentTemplate
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class EnvironmentTemplate : TypeScriptTemplateBase<object>
    {
        private IList<ConfigVariable> _configVariables = new List<ConfigVariable>();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Environment.EnvironmentTemplate.EnvironmentTemplate";

        public EnvironmentTemplate(IOutputTarget project, object model) : base(TemplateId, project, model)
        {
            project.Application.EventDispatcher.Subscribe(AngularConfigVariableRequiredEvent.EventId, HandleConfigVariableRequiredEvent);
        }

        private void HandleConfigVariableRequiredEvent(ApplicationEvent @event)
        {
            _configVariables.Add(new ConfigVariable(
                name: @event.GetValue(AngularConfigVariableRequiredEvent.VariableId),
                defaultValue: @event.GetValue(AngularConfigVariableRequiredEvent.DefaultValueId)));
        }

        public string GetEnvironmentVariables()
        {
            return _configVariables.Any() ? $@", 
  {string.Join(@",
  ", _configVariables.Select(x => $"{x.Name}: {x.DefaultValue}"))}" : "";
        }

        //      protected override TypeScriptFile CreateOutputFile()
        //      {
        //          var file = GetExistingFile() ?? base.CreateOutputFile();
        //          var variable = file.Children.First(x => x.Identifier == "environment");

        //          foreach (var configVariable in _configVariables)
        //          {
        //              var assigned = variable.Children.FirstOrDefault();//.GetAssignedValue<TypescriptObjectLiteralExpression>();
        //              if (assigned != null && assigned.Children.All(x => x.Identifier != configVariable.Name))
        //              {
        //                  assigned.InsertAfter(assigned.Children.Last(), $@"
        //{configVariable.Name}: {configVariable.DefaultValue}");
        //              }
        //          }

        //          return file;
        //      }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "environment",
                fileExtension: "ts", // Change to desired file extension.
                relativeLocation: ""
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