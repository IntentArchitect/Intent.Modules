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
        public static Bindable GetBindable(this PropertyModel model)
        {
            var stereotype = model.GetStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3");
            return stereotype != null ? new Bindable(stereotype) : null;
        }


        public static bool HasBindable(this PropertyModel model)
        {
            return model.HasStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3");
        }

        public static bool TryGetBindable(this PropertyModel model, out Bindable stereotype)
        {
            if (!HasBindable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Bindable(model.GetStereotype("12ba7bea-ceb9-44d4-8819-835fe36af7b3"));
            return true;
        }

        public static TwoWayBindable GetTwoWayBindable(this PropertyModel model)
        {
            var stereotype = model.GetStereotype("146856d2-c3c0-4ecb-9149-1685ac8e407c");
            return stereotype != null ? new TwoWayBindable(stereotype) : null;
        }


        public static bool HasTwoWayBindable(this PropertyModel model)
        {
            return model.HasStereotype("146856d2-c3c0-4ecb-9149-1685ac8e407c");
        }

        public static bool TryGetTwoWayBindable(this PropertyModel model, out TwoWayBindable stereotype)
        {
            if (!HasTwoWayBindable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TwoWayBindable(model.GetStereotype("146856d2-c3c0-4ecb-9149-1685ac8e407c"));
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

        public class TwoWayBindable
        {
            private IStereotype _stereotype;

            public TwoWayBindable(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}