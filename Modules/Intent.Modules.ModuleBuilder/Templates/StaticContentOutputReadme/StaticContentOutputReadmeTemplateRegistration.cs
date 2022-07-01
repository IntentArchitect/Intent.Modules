using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.Registrations;
using Intent.Registrations;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.TemplateRegistration.Custom", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.StaticContentOutputReadme
{
    [IntentManaged(Mode.Merge, Body = Mode.Merge, Signature = Mode.Fully)]
    public class StaticContentOutputReadmeTemplateRegistration : ITemplateRegistration
    {
        private readonly IMetadataManager _metadataManager;

        public StaticContentOutputReadmeTemplateRegistration(IMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
        }

        public string TemplateId => StaticContentOutputReadmeTemplate.TemplateId;

        [IntentManaged(Mode.Merge, Signature = Mode.Fully, Body = Mode.Ignore)]
        public void DoRegistration(ITemplateInstanceRegistry registry, IApplication applicationManager)
        {
            // This is a complicated setup but necessary in order to ensure that the correct subfolder
            // is created in the beginning and doesn't continue to pester the developer if the readme file is removed.
            var outputTargets = applicationManager.OutputTargets.Where(p => p.OutputsTemplate(TemplateId)).ToArray();
            if (outputTargets.Length > 1)
            {
                throw new NotSupportedException(
                    $"More than one output target instance of {TemplateId} was found but it's not supported.");
            }

            var outputTarget = outputTargets.FirstOrDefault();
            if (outputTarget == null)
            {
                return;
            }

            var models = _metadataManager.ModuleBuilder(applicationManager).GetStaticContentTemplateModels();
            foreach (var model in models)
            {
                if (IsFirstTimeRun(outputTarget, model))
                {
                    registry.RegisterTemplate(TemplateId,
                        project => new StaticContentOutputReadmeTemplate(project, model));
                }
            }
        }

        private bool IsFirstTimeRun(IOutputTarget outputTarget, StaticContentTemplateModel model)
        {
            var outDir = outputTarget.Location + "/" + StaticContentOutputReadmeTemplate.GetContentFolderLocation(model);
            return !Directory.Exists(outDir);
        }
    }
}