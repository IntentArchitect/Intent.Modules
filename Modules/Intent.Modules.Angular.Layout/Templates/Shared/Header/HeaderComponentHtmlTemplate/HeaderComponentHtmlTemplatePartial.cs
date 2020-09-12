using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Templates;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Templates.Shared.Header.HeaderComponentHtmlTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class HeaderComponentHtmlTemplate : IntentProjectItemTemplateBase<object>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Layout.Templates.Shared.Header.HeaderComponentHtmlTemplate";

        public HeaderComponentHtmlTemplate(IProject project, object model) : base(TemplateId, project, model)
        {
            project.Application.EventDispatcher.Subscribe(AngularModuleRouteCreatedEvent.EventId, @event =>
            {
                _mainRoutes.Add(new ModuleRoute(@event.GetValue(AngularModuleRouteCreatedEvent.ModuleName), @event.GetValue(AngularModuleRouteCreatedEvent.Route)));
            });
        }

        private List<ModuleRoute> _mainRoutes = new List<ModuleRoute>();

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig DefineDefaultFileMetadata()
        {
            return new DefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.Basic,
                fileName: "header.component",
                fileExtension: "html",
                defaultLocationInProject: "ClientApp/src/app/shared/header"
            );
        }

        private class ModuleRoute
        {
            public ModuleRoute(string moduleName, string route)
            {
                ModuleName = moduleName;
                Route = route;
            }

            public string ModuleName { get; }

            public string Route { get; }
        }
    }
}