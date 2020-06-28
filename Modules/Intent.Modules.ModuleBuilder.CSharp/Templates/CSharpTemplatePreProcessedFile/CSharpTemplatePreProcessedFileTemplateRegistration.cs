using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Registrations;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Modules.ModuleBuilder.CSharp.Api;
using Intent.Modules.ModuleBuilder.Templates.Common;
using Intent.Templates;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.ModuleBuilder.CSharp.Templates.CSharpTemplatePreProcessedFile
{
    public class CSharpTemplatePreProcessedFileTemplateRegistration : ModelTemplateRegistrationBase<CSharpTemplateModel>
    {
        private readonly IMetadataManager _metadataManager;

        public CSharpTemplatePreProcessedFileTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public override string TemplateId => "ModuleBuilder.CSharp.Templates.CSharpTemplate.T4Template.PreProcessed";

        public override ITemplate CreateTemplateInstance(IProject project, CSharpTemplateModel model)
        {
            return new TemplatePreProcessedFileTemplate(
                templateId: TemplateId,
                project: project,
                model: model,
                t4TemplateId: CSharpTemplate.CSharpTemplate.TemplateId,
                partialTemplateId: CSharpTemplatePartial.CSharpTemplatePartial.TemplateId);
        }

        public override IEnumerable<CSharpTemplateModel> GetModels(IApplication application)
        {
            return _metadataManager.GetCSharpTemplateModels(application);
        }
    }
}
