using System;
using Intent.Metadata.Models;
using Intent.ModuleBuilder.Api.Factories;
using Intent.Modules.Common;
using Intent.Modules.Common.CSharp.Templates;

namespace Intent.ModuleBuilder.Api
{
    public class ModelerModelType
    {
        private readonly ICanBeReferencedType _element;

        public ModelerModelType(ICanBeReferencedType element)
        {
            if (element.SpecializationType != "Element Settings" && 
                element.SpecializationType != "Package Settings" &&
                element.SpecializationType != "Association Source End Settings" &&
                element.SpecializationType != "Association Destination End Settings" &&
                element.SpecializationType != "Element Extension" &&
                element.SpecializationType != "Type-Definition" &&
                element.SpecializationType != "Package Extension")
            {
                throw new InvalidOperationException($"Cannot load {nameof(ModelerModelType)} from element of type {element.SpecializationType}");
            }

            _element = element;
        }

        public string Id => _element.Id;
        public string ClassName => _element.SpecializationType == "Type-Definition" ? _element.Name.ToCSharpIdentifier() : $"{_element.Name.ToCSharpIdentifier()}Model";
        public string Namespace => _element.GetStereotype("C#")?.GetProperty<string>("Namespace") ?? ParentModule.ApiNamespace;
        public string FullyQualifiedName => $"{Namespace}.{ClassName}";

        public IntentModuleModel ParentModule => new IntentModuleModel(_element.Package);
        public DesignerSettingsModel DesignerSettings => DesignerModelFactory.GetDesignerSettings(forElement: this._element);
    }
}