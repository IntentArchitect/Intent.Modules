﻿using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Modules.Common.Types.Api;
using Intent.Modules.Common.VisualStudio;
using Intent.Modules.ModuleBuilder.Api;
using Intent.Templates;

namespace Intent.Modules.ModuleBuilder.Templates.Registration.Custom
{
    partial class CustomTemplateRegistrationTemplate : CSharpTemplateBase<TemplateRegistrationModel>
    {
        public const string TemplateId = "Intent.ModuleBuilder.TemplateRegistration.Custom";

        public CustomTemplateRegistrationTemplate(IOutputTarget project, TemplateRegistrationModel model) : base(TemplateId, project, model)
        {
            if (!string.IsNullOrWhiteSpace(Model.GetModelType()?.ParentModule.NuGetPackageId))
            {
                AddNugetDependency(new NugetPackageInfo(Model.GetModelType()?.ParentModule.NuGetPackageId, Model.GetModelType()?.ParentModule.NuGetPackageVersion));
            }
        }

        public IList<string> OutputFolder => Model.GetFolderPath().Select(x => x.Name).Concat(new[] { Model.Name }).ToList();
        public string FolderPath => string.Join("/", OutputFolder);
        public string FolderNamespace => string.Join(".", OutputFolder);

        protected override CSharpDefaultFileConfig DefineFileConfig()
        {
            return new CSharpDefaultFileConfig(
                className: $"{Model.Name}Registration",
                @namespace: $"{OutputTarget.GetNamespace()}.{FolderNamespace}",
                relativeLocation: $"{FolderPath}");
        }

        //protected override RoslynDefaultFileMetadata DefineRoslynDefaultFileMetadata()
        //{
        //    return new RoslynDefaultFileMetadata(
        //        overwriteBehaviour: OverwriteBehaviour.Always,
        //        fileName: "${Model.Name}Registration",
        //        fileExtension: "cs",
        //        defaultLocationInProject: "${FolderPath}/${Model.Name}",
        //        className: "${Model.Name}Registration",
        //        @namespace: "${Project.Name}.${FolderNamespace}.${Model.Name}"
        //    );
        //}

        private string GetTemplateNameForTemplateId()
        {
            return Model.Name.Replace("Registrations", "Template");
        }
    }
}
