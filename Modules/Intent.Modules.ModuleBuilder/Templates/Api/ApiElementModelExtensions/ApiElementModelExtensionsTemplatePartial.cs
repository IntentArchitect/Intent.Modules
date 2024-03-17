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

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Merge)]
    partial class ApiElementModelExtensionsTemplate : CSharpTemplateBase<ExtensionModel>, IDeclareUsings
    {

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions";

        [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
        public ApiElementModelExtensionsTemplate(IOutputTarget outputTarget, ExtensionModel model = null) : base(TemplateId, outputTarget, model)
        {
            AddNugetDependency(IntentNugetPackages.IntentModulesCommon);

            foreach (var module in Model.StereotypeDefinitions
                .SelectMany(x => x.TargetElements)
                .Distinct()
                .Select(x => new IntentModuleModel(x.Package))
                .Distinct()
                .Where(x => !string.IsNullOrWhiteSpace(x.NuGetPackageId) && !string.IsNullOrWhiteSpace(x.NuGetPackageVersion) &&
                            outputTarget.GetProject().Name != x.NuGetPackageId))
            {
                AddNugetDependency(module.NuGetPackageId, module.NuGetPackageVersion);
            }
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Type.ApiClassName}StereotypeExtensions",
                @namespace: new IntentModuleModel(Model.StereotypeDefinitions.First().Package).ApiNamespace);
        }

        public string ModelClassName => Model.Type.ApiClassName;

        public IEnumerable<string> DeclareUsings()
        {
            yield return Model.Type.ApiNamespace;
        }

        private static string ForcePluralize(string value)
        {
            var pluralized = value.Pluralize();

            return value == pluralized
                ? $"{value}s"
                : pluralized;
        }
    }
}