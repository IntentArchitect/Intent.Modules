using System;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class ModelerModelType : IModelerModelType
    {
        public const string SpecializationType = "Model Type";

        private readonly ILiteral _element;

        public ModelerModelType(ILiteral element)
        {
            if (element.SpecializationType != SpecializationType)
            {
                throw new InvalidOperationException($"Cannot load {nameof(ModelerModelType)} from element of type {element.SpecializationType}");
            }

            _element = element;
        }

        public string Id => _element.Id;
        public string Name => _element.Name;
        public string Namespace => _element.GetStereotypeProperty<string>("Model Type Settings", "Namespace");
        public string LoadMethod => _element.GetStereotypeProperty<string>("Model Type Settings", "Load Method");
    }
}