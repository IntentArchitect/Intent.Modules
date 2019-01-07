using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common.Templates;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.IdentityServer4.Templates.IdentityConfig
{
    partial class IdentityConfig : IntentRoslynProjectItemTemplateBase<object>
    {
        public const string TemplateId = "IdentityServer4.IdentityConfig";

        public IdentityConfig(IProject project, object model) : base(TemplateId, project, model)
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
                fileName: "IdentityConfig",
                fileExtension: "cs",
                defaultLocationInProject: "IdentityConfig",
                className: "IdentityConfig",
                @namespace: "${Project.ProjectName}"
            );
        }
    }
}