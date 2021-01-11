using System.Collections.Generic;
using System.Linq;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.ModuleBuilder.Api;
using Intent.RoslynWeaver.Attributes;
using Intent.Templates;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.ModuleBuilder.CSharp.Templates.CSharpTemplatePartial", Version = "1.0")]

namespace Intent.Modules.ModuleBuilder.Templates.Api.ApiElementModelExtensions
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    partial class ApiElementModelExtensionsTemplate : CSharpTemplateBase<ExtensionModel>, IDeclareUsings
    {

        [IntentManaged(Mode.Fully)]
        public const string TemplateId = "Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions";

        public ApiElementModelExtensionsTemplate(IOutputTarget project, ExtensionModel model) : base(TemplateId, project, model)
        {
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

    public class ExtensionModel
    {
        public IEnumerable<IStereotypeDefinition> StereotypeDefinitions { get; }
        public ExtensionModelType Type { get; set; }

        public ExtensionModel(ExtensionModelType type, IEnumerable<IStereotypeDefinition> stereotypeDefinitions)
        {
            StereotypeDefinitions = stereotypeDefinitions;
            Type = type;
        }

        //public ElementSettingsModel Type { get; }

        //public ExtensionModel(ElementSettingsModel element, IEnumerable<IStereotypeDefinition> stereotypeDefinitions)
        //{
        //    StereotypeDefinitions = stereotypeDefinitions;
        //    Type = element;
        //}
    }

    public class ExtensionModelType
    {
        private readonly IElement _element;
        public string Name => _element.Name;
        public string ApiNamespace => new IntentModuleModel(_element.Package).ApiNamespace;
        public string ApiClassName => $"{Name.ToCSharpIdentifier()}Model";

        public ExtensionModelType(IElement element)
        {
            _element = element;
        }
    }
}