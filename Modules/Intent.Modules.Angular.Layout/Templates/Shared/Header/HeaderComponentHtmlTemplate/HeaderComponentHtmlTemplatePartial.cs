using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Templates;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Editor;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.ProjectItemTemplate.Partial", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Templates.Shared.Header.HeaderComponentHtmlTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class HeaderComponentHtmlTemplate : IntentProjectItemTemplateBase<object>
    {
        private List<ModuleRoute> _mainRoutes = new List<ModuleRoute>();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Layout.Templates.Shared.Header.HeaderComponentHtmlTemplate";

        public HeaderComponentHtmlTemplate(IProject project, object model) : base(TemplateId, project, model)
        {
            project.Application.EventDispatcher.Subscribe(AngularModuleRouteCreatedEvent.EventId, @event =>
            {
                _mainRoutes.Add(new ModuleRoute(@event.GetValue(AngularModuleRouteCreatedEvent.ModuleName), @event.GetValue(AngularModuleRouteCreatedEvent.Route)));
            });
        }

        public override string RunTemplate()
        {
            var file = GetExistingFile() ?? base.RunTemplate();
            var html = new HtmlDocument();
            html.LoadHtml(file);
            var navbar = html.DocumentNode.SelectSingleNode("//ul[@intent-managed='navbar']");
            if (navbar != null)
            {
                foreach (var moduleRoute in _mainRoutes)
                {
                    if (navbar.SelectSingleNode("//li[a[@routerLink='/users']]") == null)
                    {
                        navbar.AppendChild(HtmlNode.CreateNode($@"
      <li class=""nav-item active"">
        <a class=""nav-link"" routerLink=""/{moduleRoute.Route}"">{moduleRoute.ModuleName}</a>
      </li>"));
                    }
                }
            }

            using (StringWriter writer = new StringWriter())
            {
                html.Save(writer);
                return writer.ToString();
            }
        }

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

        private string GetExistingFile()
        {
            var metadata = GetMetadata();
            var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());
            return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : null;
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