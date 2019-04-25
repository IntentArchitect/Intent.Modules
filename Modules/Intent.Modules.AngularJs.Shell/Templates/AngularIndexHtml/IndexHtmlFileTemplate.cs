using HtmlAgilityPack;
using Intent.Modules.Bower.Contracts;
using Intent.Modules.Constants;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Eventing;
using Intent.Templates
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Modules.Bower;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;

namespace Intent.Modules.AngularJs.Shell.Templates.AngularIndexHtml
{
    public class IndexHtmlFileTemplate : IntentProjectItemTemplateBase<object>
    {
        public const string Identifier = "Intent.AngularJs.Shell.IndexHtml";

        private IList<string> BowerCssFiles => GetOrderedPackages().SelectMany(x => x.CssSources).ToList();

        private IList<string> BowerJsFiles => GetOrderedPackages().SelectMany(x => x.JsSources).ToList();

        private readonly IList<string> _eventedJsFiles = new List<string>();

        public IndexHtmlFileTemplate(IProject project, IApplicationEventDispatcher eventDispatcher) : base(Identifier, project, null)
        {
            eventDispatcher.Subscribe(ApplicationEvents.IndexHtml_JsFileAvailable, Handle);
        }

        private void Handle(ApplicationEvent @event)
        {
            _eventedJsFiles.Add(@event.GetValue("Src"));
        }

        public override string TransformText()
        {
            var location = FileMetaData.GetFullLocationPathWithFileName();

            var doc = LoadOrCreateWebConfig(location);

            foreach (var cssHref in BowerCssFiles.Reverse())
            {
                var head = doc.DocumentNode.SelectSingleNode("//html//head");
                if (head.SelectSingleNode($"//link[@href='./lib/{cssHref}']") == null)
                {
                    var cssLink = doc.CreateElement("link");
                    cssLink.Attributes.Add(doc.CreateAttribute("rel", "stylesheet"));
                    cssLink.Attributes.Add(doc.CreateAttribute("href", $"./lib/{cssHref}"));
                    head.AppendChild(cssLink);
                    head.InsertBefore(HtmlNode.CreateNode("    "), cssLink);
                }
            }

            var body = doc.DocumentNode.SelectSingleNode("//html//body");
            //foreach (var jsSrc in BowerJsFiles)
            //{
            //    if (body.SelectSingleNode($"//script[@src='./lib/{jsSrc}']") != null)
            //    {
            //        body.RemoveChild(body.SelectSingleNode($"//script[@src='./lib/{jsSrc}']"));
            //    }
            //}
            var newLine = HtmlNode.CreateNode("\r\n    ");
            var firstScript = body.SelectSingleNode($"//script[1]");
            foreach (var jsSrc in BowerJsFiles)
            {
                if (body.SelectSingleNode($"//script[@src='./lib/{jsSrc}']") == null && body.SelectSingleNode($"//comment()[contains(., './lib/{jsSrc}')]") == null)
                {
                    var jsScript = doc.CreateElement("script");
                    jsScript.Attributes.Add(doc.CreateAttribute("type", "text/javascript"));
                    jsScript.Attributes.Add(doc.CreateAttribute("src", $"./lib/{jsSrc}"));
                    body.InsertBefore(jsScript, firstScript);
                    body.InsertBefore(newLine, firstScript);
                }
            }
            foreach (var jsSrc in _eventedJsFiles)
            {
                if (body.SelectSingleNode($"//script[@src='{jsSrc}']") == null)
                {
                    var jsScript = doc.CreateElement("script");
                    jsScript.Attributes.Add(doc.CreateAttribute("type", "text/javascript"));
                    jsScript.Attributes.Add(doc.CreateAttribute("src", $"{jsSrc}"));
                    body.AppendChild(jsScript);
                }
            }
            return doc.DocumentNode.InnerHtml;
        }

        public override ITemplateFileConfig DefineDefaultFileMetaData()
        {
            return new DefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                codeGenType: CodeGenType.UserControlledWeave,
                fileName: "index",
                fileExtension: "html",
                defaultLocationInProject: "wwwroot"
                );
        }

        private HtmlDocument LoadOrCreateWebConfig(string filePath)
        {
            var doc = new HtmlDocument();
            if (File.Exists(filePath))
            {
                doc.Load(filePath);
            }
            else
            {
                doc.LoadHtml($@"<html lang=""en"" ng-app=""App"" ng-strict-di="""">
<head>
    <title>{ Project.ApplicationName() }</title>
    <meta charset=""utf-8"" />
	<meta name=""viewport"" content=""width=device-width, initial-scale=1, maximum-scale=1""/>
</head>
<body ng-cloak="""">
    <div ui-view=""""></div>
    <script type=""text/javascript"" src=""./js/app.js""></script>
</body>
</html>");
            }
            return doc;
        }

        private List<IBowerPackageInfo> GetOrderedPackages()
        {
            // The world's dodgiest dependency order system... I don't even think it will always work. Sorry guys :'(
            var orderedList = new List<IBowerPackageInfo>();
            foreach (var bowerPackage in Project.BowerPackages())
            {
                foreach (var package in FlattenPackageDependencies(bowerPackage))
                {
                    if (!orderedList.Contains(package))
                    {
                        orderedList.Add(package);
                    }
                }
            }
            return orderedList;
        }

        private List<IBowerPackageInfo> FlattenPackageDependencies(IBowerPackageInfo package)
        {
            var depenencies = package.Dependencies.SelectMany(FlattenPackageDependencies).ToList();
            depenencies.Add(package);
            return depenencies;
        }
    }
}


