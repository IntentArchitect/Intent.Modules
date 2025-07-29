using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.DocumentDB.Api
{
    public static class AssociationTargetEndModelStereotypeExtensions
    {
        public static FieldSettings GetFieldSettings(this AssociationTargetEndModel model)
        {
            var stereotype = model.GetStereotype(FieldSettings.DefinitionId);
            return stereotype != null ? new FieldSettings(stereotype) : null;
        }


        public static bool HasFieldSettings(this AssociationTargetEndModel model)
        {
            return model.HasStereotype(FieldSettings.DefinitionId);
        }

        public static bool TryGetFieldSettings(this AssociationTargetEndModel model, out FieldSettings stereotype)
        {
            if (!HasFieldSettings(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new FieldSettings(model.GetStereotype(FieldSettings.DefinitionId));
            return true;
        }

        public class FieldSettings
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "fb47f1e4-447b-4a67-947d-590fc24c20c1";

            public FieldSettings(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

        }

    }
}