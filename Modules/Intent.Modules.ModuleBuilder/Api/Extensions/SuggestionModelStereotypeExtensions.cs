using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.ModuleBuilder.Api
{
    public static class SuggestionModelStereotypeExtensions
    {
        public static Settings GetSettings(this SuggestionModel model)
        {
            var stereotype = model.GetStereotype(Settings.DefinitionId);
            return stereotype != null ? new Settings(stereotype) : null;
        }


        public static bool HasSettings(this SuggestionModel model)
        {
            return model.HasStereotype(Settings.DefinitionId);
        }

        public static bool TryGetSettings(this SuggestionModel model, out Settings stereotype)
        {
            if (!HasSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Settings(model.GetStereotype(Settings.DefinitionId));
            return true;
        }

        public class Settings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "daf7d9b3-9c10-4286-b30e-7edb2e62f10e";

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public LocationsOptions[] Locations()
            {
                return _stereotype.GetProperty<string[]>("Locations")?.Select(x => new LocationsOptions(x)).ToArray() ?? new LocationsOptions[0];
            }

            public string FilterFunction()
            {
                return _stereotype.GetProperty<string>("Filter Function");
            }

            public string DisplayFunction()
            {
                return _stereotype.GetProperty<string>("Display Function");
            }

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string Script()
            {
                return _stereotype.GetProperty<string>("Script");
            }

            public IElement[] Dependencies()
            {
                return _stereotype.GetProperty<IElement[]>("Dependencies") ?? new IElement[0];
            }

            public int? OrderPriority()
            {
                return _stereotype.GetProperty<int?>("Order Priority");
            }

            public class LocationsOptions
            {
                public readonly string Value;

                public LocationsOptions(string value)
                {
                    Value = value;
                }

                public LocationsOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "Diagram":
                            return LocationsOptionsEnum.Diagram;
                        case "Model":
                            return LocationsOptionsEnum.Model;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsDiagram()
                {
                    return Value == "Diagram";
                }
                public bool IsModel()
                {
                    return Value == "Model";
                }
            }

            public enum LocationsOptionsEnum
            {
                Diagram,
                Model
            }
        }

    }
}