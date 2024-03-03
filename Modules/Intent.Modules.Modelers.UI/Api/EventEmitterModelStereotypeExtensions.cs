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
    public static class EventEmitterModelStereotypeExtensions
    {
        public static Bindable GetBindable(this EventEmitterModel model)
        {
            var stereotype = model.GetStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3");
            return stereotype != null ? new Bindable(stereotype) : null;
        }


        public static bool HasBindable(this EventEmitterModel model)
        {
            return model.HasStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3");
        }

        public static bool TryGetBindable(this EventEmitterModel model, out Bindable stereotype)
        {
            if (!HasBindable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Bindable(model.GetStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3"));
            return true;
        }

        public class Bindable
        {
            private IStereotype _stereotype;

            public Bindable(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}