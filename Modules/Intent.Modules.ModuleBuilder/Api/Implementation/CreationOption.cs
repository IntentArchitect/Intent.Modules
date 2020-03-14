using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modules.Common;

namespace Intent.Modules.ModuleBuilder.Api
{
    internal class CreationOption : ICreationOption
    {
        private readonly IElement _element;
        public const string RequiredSpecializationType = "Creation Option";

        public CreationOption(IElement element)
        {
            if (element.SpecializationType != RequiredSpecializationType)
            {
                throw new ArgumentException($"Invalid element [{element}]");
            }

            _element = element;

            SpecializationType = element.TypeReference.Element.Name;
            Text = element.Name;
            Shortcut = element.TypeReference.Element.GetStereotypeProperty<string>("Default Creation Options", "Shortcut");
            DefaultName = element.TypeReference.Element.GetStereotypeProperty<string>("Default Creation Options", "Default Name") ?? $"New{element.TypeReference.Element.Name.Replace(" " ,"")}";
            Icon = IconModel.CreateIfSpecified(element.TypeReference.Element.GetStereotype("Icon (Full)"));
            Type = element.TypeReference.Element;
            AllowMultiple = element.GetStereotypeProperty("Creation Options", "Allow Multiple", true);
        }

        public string Id => _element.Id;
        public IEnumerable<IStereotype> Stereotypes => _element.Stereotypes;
        public string Name => _element.Name;

        public string SpecializationType { get; }

        public string Text { get; }

        public string Shortcut { get; }

        public string DefaultName { get; }

        public IconModel Icon { get; }

        public IElement Type { get; }

        public bool AllowMultiple { get; }

        public override string ToString()
        {
            return $"{nameof(SpecializationType)} = '{SpecializationType}', " +
                   $"{nameof(Text)} = '{Text}'";
        }
    }
}