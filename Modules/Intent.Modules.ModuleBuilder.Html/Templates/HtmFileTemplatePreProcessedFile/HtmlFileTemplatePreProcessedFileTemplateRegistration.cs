using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.ModuleBuilder.Api;
using Intent.ModuleBuilder.Html.Api;
using Intent.Modules.ModuleBuilder.Html.Templates.HtmlFile;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.Html.Templates.HtmFileTemplatePreProcessedFile
{
    public class HtmlFileTemplatePreProcessedFileTemplateRegistration : FilePerModelTemplateRegistration<HtmlFileTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public HtmlFileTemplatePreProcessedFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "ModuleBuilder.Html.Templates.HtmlFileTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IOutputTarget project, HtmlFileTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: HtmlFileTemplate.TemplateId,
                partialTemplateId: HtmlFileTemplatePartial.HtmlFileTemplatePartialTemplate.TemplateId);
        }

        public override IEnumerable<HtmlFileTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.ModuleBuilder(application).GetHtmlFileTemplateModels();
        }
    }
}
