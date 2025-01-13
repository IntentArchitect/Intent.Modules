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
            var stereotype = model.GetStereotype(Bindable.DefinitionId);
            return stereotype != null ? new Bindable(stereotype) : null;
        }


        public static bool HasBindable(this PropertyModel model)
        {
            return model.HasStereotype(Bindable.DefinitionId);
        }

        public static bool TryGetBindable(this PropertyModel model, out Bindable stereotype)
        {
            if (!HasBindable(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Bindable(model.GetStereotype(Bindable.DefinitionId));
            return true;
        }

        public static RouteParameter GetRouteParameter(this PropertyModel model)
        {
            var stereotype = model.GetStereotype(RouteParameter.DefinitionId);
            return stereotype != null ? new RouteParameter(stereotype) : null;
        }


        public static bool HasRouteParameter(this PropertyModel model)
        {
            return model.HasStereotype(RouteParameter.DefinitionId);
        }

        public static bool TryGetRouteParameter(this PropertyModel model, out RouteParameter stereotype)
        {
            if (!HasRouteParameter(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RouteParameter(model.GetStereotype(RouteParameter.DefinitionId));
            return true;
        }

        public static QueryParameter GetQueryParameter(this PropertyModel model)
        {
            var stereotype = model.GetStereotype(QueryParameter.DefinitionId);
            return stereotype != null ? new QueryParameter(stereotype) : null;
        }


        public static bool HasQueryParameter(this PropertyModel model)
        {
            return model.HasStereotype(QueryParameter.DefinitionId);
        }

        public static bool TryGetQueryParameter(this PropertyModel model, out QueryParameter stereotype)
        {
            if (!HasQueryParameter(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new QueryParameter(model.GetStereotype(QueryParameter.DefinitionId));
            return true;
        }

        public class Bindable
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "12ba7bea-ceb9-44d4-8819-835fe36af7b3";

            public Bindable(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class RouteParameter
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f324c4ea-bac2-450d-b1b1-cc7f09ca3472";

            public RouteParameter(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class QueryParameter
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5c99275d-be5b-4bc9-849a-a283cdb80b75";

            public QueryParameter(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}