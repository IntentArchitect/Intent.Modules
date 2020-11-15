using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Angular.Layout.Html;
using Intent.Modules.Angular.Templates;
using Intent.Modules.Angular.Templates.Component.AngularComponentTsTemplate;
using Intent.Modules.Common;
using Intent.Modules.Common.Html.Templates;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;
using Intent.Modules.Angular.Layout.Api;


[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("ModuleBuilder.Html.Templates.HtmlFileTemplatePartial", Version = "1.0")]

namespace Intent.Modules.Angular.Layout.Templates.Shared.Header.HeaderComponentHtmlTemplate
{
    [IntentManaged(Mode.Merge)]
    partial class HeaderComponentHtmlTemplate : HtmlTemplateBase<object>
    {
        private List<ModuleRoute> _mainRoutes = new List<ModuleRoute>();

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Angular.Layout.Shared.Header.HeaderComponentHtmlTemplate";

        public HeaderComponentHtmlTemplate(IOutputTarget project) : base(TemplateId, project, null)
        {
            project.Application.EventDispatcher.Subscribe(AngularModuleRouteCreatedEvent.EventId, @event =>
            {
                _mainRoutes.Add(new ModuleRoute(@event.GetValue(AngularModuleRouteCreatedEvent.ModuleName), @event.GetValue(AngularModuleRouteCreatedEvent.Route)));
            });
        }

        //  public override string RunTemplate()
        //  {
        //      var file = GetExistingFile() ?? base.RunTemplate();
        //      var html = new HtmlDocument()
        //      {
        //          OptionOutputOriginalCase = true,
        //          OptionWriteEmptyNodes = true
        //      };
        //      html.LoadHtml(file);
        //      var navbar = html.DocumentNode.SelectSingleNode("//ul[@intent-managed='navbar']");
        //      if (navbar != null)
        //      {
        //          foreach (var moduleRoute in _mainRoutes)
        //          {
        //              if (navbar.SelectSingleNode($"//a[@routerlink='/{moduleRoute.Route}']") == null)
        //              {
        //                  navbar.AppendChild($@"
        //<li class=""nav-item active"">
        //  <a class=""nav-link"" routerLink=""/{moduleRoute.Route}"">{moduleRoute.ModuleName}</a>
        //</li>");
        //              }
        //          }
        //      }

        //      using (StringWriter writer = new StringWriter())
        //      {
        //          html.Save(writer);
        //          return writer.ToString();
        //      }
        //  }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new HtmlFileConfig(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "header.component",
                relativeLocation: ""
            );
        }

        //private string GetExistingFile()
        //{
        //    var metadata = GetMetadata();
        //    var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());
        //    return File.Exists(fullFileName) ? File.ReadAllText(fullFileName) : null;
        //}

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