using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Api
{
    public static class PropertyModelStereotypeExtensions
    {
        public static Input GetInput(this PropertyModel model)
        {
            var stereotype = model.GetStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3");
            return stereotype != null ? new Input(stereotype) : null;
        }


        public static bool HasInput(this PropertyModel model)
        {
            return model.HasStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3");
        }

        public static bool TryGetInput(this PropertyModel model, out Input stereotype)
        {
            if (!HasInput(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Input(model.GetStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3"));
            return true;
        }

        public static Output GetOutput(this PropertyModel model)
        {
            var stereotype = model.GetStereotype("228b4b42-0ee0-407a-b5e8-6c2a1335baef");
            return stereotype != null ? new Output(stereotype) : null;
        }


        public static bool HasOutput(this PropertyModel model)
        {
            return model.HasStereotype("228b4b42-0ee0-407a-b5e8-6c2a1335baef");
        }

        public static bool TryGetOutput(this PropertyModel model, out Output stereotype)
        {
            if (!HasOutput(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Output(model.GetStereotype("228b4b42-0ee0-407a-b5e8-6c2a1335baef"));
            return true;
        }

        public class Input
        {
            private IStereotype _stereotype;

            public Input(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class Output
        {
            private IStereotype _stereotype;

            public Output(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}