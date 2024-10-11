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
    public static class DocumentationTopicModelStereotypeExtensions
    {
        public static Settings GetSettings(this DocumentationTopicModel model)
        {
            var stereotype = model.GetStereotype(Settings.DefinitionId);
            return stereotype != null ? new Settings(stereotype) : null;
        }


        public static bool HasSettings(this DocumentationTopicModel model)
        {
            return model.HasStereotype(Settings.DefinitionId);
        }

        public static bool TryGetSettings(this DocumentationTopicModel model, out Settings stereotype)
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
            public const string DefinitionId = "83dfd8ae-5086-4e41-94c2-7110d5a49b4c";

            public Settings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Description()
            {
                return _stereotype.GetProperty<string>("Description");
            }

            public IIconModel Icon()
            {
                return _stereotype.GetProperty<IIconModel>("Icon");
            }

            public string Href()
            {
                return _stereotype.GetProperty<string>("Href");
            }

            public IElement[] Designers()
            {
                return _stereotype.GetProperty<IElement[]>("Designers") ?? new IElement[0];
            }

            public IElement[] Elements()
            {
                return _stereotype.GetProperty<IElement[]>("Elements") ?? new IElement[0];
            }

            public string Tags()
            {
                return _stereotype.GetProperty<string>("Tags");
            }

        }

    }
}