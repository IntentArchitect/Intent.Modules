using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ModuleSettingsExtensionsTemplate : CSharpTemplateBase<IntentModuleModel>
    {
        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Settings.ModuleSettingsExtensions";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ModuleSettingsExtensionsTemplate(IOutputTarget outputTarget, IntentModuleModel model) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);
            foreach (var module in Model.SettingsExtensions.Select(x => x.TypeReference.Element)
                .Distinct()
                .Select(x => new IntentModuleModel(x.Package))
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x.NuGetPackageId) && !string.IsNullOrWhiteSpace(x.NuGetPackageVersion) &&
                            outputTarget.GetProject().Name != x.NuGetPackageId))
            {
                AddNugetDependency(module.NuGetPackageId, module.NuGetPackageVersion);
                AddUsing($"{module.NuGetPackageId}.Settings");
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"ModuleSettingsExtensions",
                @namespace: $"{this.GetNamespace()}",
                relativeLocation: $"{this.GetFolderPath()}");
        }


        public override bool CanRunTemplate()
        {
            return Model.SettingsGroups.Any() || Model.SettingsExtensions.Any();
        }
    }
}