using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AspNet.WebApi.Templates.WebApiBadHttpRequestException
{
    partial class WebApiBadHttpRequestExceptionTemplate : IntentRoslynProjectItemTemplateBase<object>, ITemplate
    {
        public const string Identifier = "Intent.AspNet.WebApi.BadHttpRequestException";
        public WebApiBadHttpRequestExceptionTemplate(IProject project)
            : base (Identifier, project, null)
        {
        }

        public override RoslynMergeConfig ConfigureRoslynMerger()
        {
            return new RoslynMergeConfig(new TemplateMetaData(Id, "1.0"));
        }

        protected override RoslynDefaultFileMetaData DefineRoslynDefaultFileMetaData()
        {
            return new RoslynDefaultFileMetaData(
                overwriteBehaviour: OverwriteBehaviour.Always,
                fileName: "BadHttpRequestException",
                fileExtension: "cs",
                defaultLocationInProject: "App_Start",
                className: "BadHttpRequestException",
                @namespace: "${Project.Name}"
                );
        }
    }
}
