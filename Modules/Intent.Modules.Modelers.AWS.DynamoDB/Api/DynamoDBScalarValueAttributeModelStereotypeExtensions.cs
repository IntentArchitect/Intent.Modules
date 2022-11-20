using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    public static class DynamoDBScalarValueAttributeModelStereotypeExtensions
    {
        public static HashKey GetHashKey(this DynamoDBScalarValueAttributeModel model)
        {
            var stereotype = model.GetStereotype("Hash Key");
            return stereotype != null ? new HashKey(stereotype) : null;
        }


        public static bool HasHashKey(this DynamoDBScalarValueAttributeModel model)
        {
            return model.HasStereotype("Hash Key");
        }

        public static bool TryGetHashKey(this DynamoDBScalarValueAttributeModel model, out HashKey stereotype)
        {
            if (!HasHashKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new HashKey(model.GetStereotype("Hash Key"));
            return true;
        }

        public static SortKey GetSortKey(this DynamoDBScalarValueAttributeModel model)
        {
            var stereotype = model.GetStereotype("Sort Key");
            return stereotype != null ? new SortKey(stereotype) : null;
        }


        public static bool HasSortKey(this DynamoDBScalarValueAttributeModel model)
        {
            return model.HasStereotype("Sort Key");
        }

        public static bool TryGetSortKey(this DynamoDBScalarValueAttributeModel model, out SortKey stereotype)
        {
            if (!HasSortKey(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new SortKey(model.GetStereotype("Sort Key"));
            return true;
        }

        public class HashKey
        {
            private IStereotype _stereotype;

            public HashKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class SortKey
        {
            private IStereotype _stereotype;

            public SortKey(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}