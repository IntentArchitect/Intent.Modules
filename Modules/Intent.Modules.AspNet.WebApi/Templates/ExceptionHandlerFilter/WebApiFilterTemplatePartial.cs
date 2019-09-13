using Intent.Engine;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.VisualStudio;

namespace Intent.Modules.AspNet.WebApi.Templates.ExceptionHandlerFilter
{
    partial class WebApiFilterTemplate : IntentRoslynProjectItemTemplateBase<object>, IDeclareUsings, IHasAssemblyDependencies
    {
        public const string TemplateId = "Intent.AspNet.WebApi.ExceptionHandlerFilter";

        public WebApiFilterTemplate(string templateId, IProject project) 
            : base(templateId, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetadata(Id, "1.0"));
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

        protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        {
            return new RoslynDefaultFileMetadata(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "ExceptionHandlerFilter",
                fileExtension: "cs",
                defaultLocationInProject: "Filters",
                className: "ExceptionHandlerFilter",
                @namespace: "${Project.ProjectName}.Filters"
            );
        }

        public IEnumerable<IAssemblyReference> GetAssemblyDependencies()
        {
            return new[]
            {
                new GacAssemblyReference("System.Net.Http"),
            };
        }
    }
}
