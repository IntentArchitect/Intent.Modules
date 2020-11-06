using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;
using Intent.Modules.Common.Templates;
using Intent.Modules.ModuleBuilder.Api.Factories;
using Intent.Modules.ModuleBuilder.Helpers;

namespace Intent.Modules.ModuleBuilder.Api
{
    public class ModelerModelType
    {
        public const string RequiredSpecializationType = "Element Settings";

        private readonly IElement _element;

        public ModelerModelType(IElement element, string requiredType = RequiredSpecializationType)
        {
            if (element.SpecializationType != requiredType)
            {
                throw new InvalidOperationException($"Cannot load {nameof(ModelerModelType)} from element of type {element.SpecializationType}");
            }

            _element = element;
        }

        public string Id => _element.Id;
        public string ClassName => $"{_element.Name.ToCSharpIdentifier()}Model";

        public IntentModuleModel ParentModule => new IntentModuleModel(_element.Package);
        public DesignerSettingsModel DesignerSettings => DesignerModelFactory.GetDesignerSettings(forElement: this._element);
    }
}