using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.ModuleBuilder.Api;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp;
using Intent.Modules.Common.CSharp.Templates;
using Intent.RoslynWeaver.Attributes;

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
                .Where(x => !string.IsNullOrWhiteSpace(x.NuGetPackageId) && !string.IsNullOrWhiteSpace(x.NuGetPackageVersion)))
            {
                AddNugetDependency(module.NuGetPackageId, module.NuGetPackageVersion);
            }
        }

        protected override CSharpFileConfig DefineFileConfig()
        {
            return new CSharpFileConfig(
                className: $"{Model.Type.ApiClassName}Extensions",
                @namespace: new IntentModuleModel(Model.StereotypeDefinitions.First().Package).ApiNamespace);
        }

        public string ModelClassName => Model.Type.ApiClassName;

        public IEnumerable<string> DeclareUsings()
        {
            yield return Model.Type.ApiNamespace;
        }
    }
}