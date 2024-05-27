using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.UI.Core.Api
{
    public static class TableModelStereotypeExtensions
    {
        public static Interaction GetInteraction(this TableModel model)
        {
            var stereotype = model.GetStereotype(Interaction.DefinitionId);
            return stereotype != null ? new Interaction(stereotype) : null;
        }


        public static bool HasInteraction(this TableModel model)
        {
            return model.HasStereotype(Interaction.DefinitionId);
        }

        public static bool TryGetInteraction(this TableModel model, out Interaction stereotype)
        {
            if (!HasInteraction(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Interaction(model.GetStereotype(Interaction.DefinitionId));
            return true;
        }

        public static Pagination GetPagination(this TableModel model)
        {
            var stereotype = model.GetStereotype(Pagination.DefinitionId);
            return stereotype != null ? new Pagination(stereotype) : null;
        }


        public static bool HasPagination(this TableModel model)
        {
            return model.HasStereotype(Pagination.DefinitionId);
        }

        public static bool TryGetPagination(this TableModel model, out Pagination stereotype)
        {
            if (!HasPagination(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Pagination(model.GetStereotype(Pagination.DefinitionId));
            return true;
        }

        public class Interaction
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "74533b82-2078-44e1-84bb-4cd34d00ef16";

            public Interaction(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string OnRowClick()
            {
                return _stereotype.GetProperty<string>("On Row Click");
            }

        }

        public class Pagination
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "f0663cd4-da7c-43cc-9cda-bbc7923d5431";

            public Pagination(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string CurrentPage()
            {
                return _stereotype.GetProperty<string>("Current Page");
            }

            public string PagesCount()
            {
                return _stereotype.GetProperty<string>("Pages Count");
            }

            public string OnPageChanged()
            {
                return _stereotype.GetProperty<string>("On Page Changed");
            }

        }

    }
}