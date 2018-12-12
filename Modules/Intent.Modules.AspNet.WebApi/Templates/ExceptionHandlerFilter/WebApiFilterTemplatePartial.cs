using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.AspNet.WebApi.Templates.ExceptionHandlerFilter
{
    partial class WebApiFilterTemplate : IntentRoslynProjectItemTemplateBase<object>, IDeclareUsings
    {
        public const string TemplateId = "Intent.AspNet.WebApi.ExceptionHandlerFilter";

        public WebApiFilterTemplate(string templateId, IProject project) 
            : base(templateId, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        public IEnumerable<string> DeclareUsings()
        {
            return new[] 
            {
                "System.Net",
                "System.Net.Http",
                "System.Web.Http.Filters"
            };
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "ExceptionHandlerFilter",
                fileExtension: "cs",
                defaultLocationInProject: "Filters",
                className: "ExceptionHandlerFilter",
                @namespace: "${Project.ProjectName}.Filters"
            );
        }
    }
}
