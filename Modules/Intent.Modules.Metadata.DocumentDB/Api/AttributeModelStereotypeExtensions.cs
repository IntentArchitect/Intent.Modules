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
    public static class AttributeModelStereotypeExtensions
    {
        public static ForeignKey GetForeignKey(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Foreign Key");
            return stereotype != null ? new ForeignKey(stereotype) : null;
        }


        public static bool HasForeignKey(this AttributeModel model)
        {
            return model.HasStereotype("Foreign Key");
        }

        public static bool TryGetForeignKey(this AttributeModel model, out ForeignKey stereotype)
        {
            if (!HasForeignKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new ForeignKey(model.GetStereotype("Foreign Key"));
            return true;
        }

        public static PrimaryKey GetPrimaryKey(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Primary Key");
            return stereotype != null ? new PrimaryKey(stereotype) : null;
        }


        public static bool HasPrimaryKey(this AttributeModel model)
        {
            return model.HasStereotype("Primary Key");
        }

        public static bool TryGetPrimaryKey(this AttributeModel model, out PrimaryKey stereotype)
        {
            if (!HasPrimaryKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new PrimaryKey(model.GetStereotype("Primary Key"));
            return true;
        }

        public class ForeignKey
        {
            private IStereotype _stereotype;

            public ForeignKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement Association()
            {
                return _stereotype.GetProperty<IElement>("Association");
            }

        }

        public class PrimaryKey
        {
            private IStereotype _stereotype;

            public PrimaryKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}