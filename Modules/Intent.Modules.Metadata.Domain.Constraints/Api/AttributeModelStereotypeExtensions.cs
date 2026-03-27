using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.Domain.Constraints.Api
{
    public static class AttributeModelStereotypeExtensions
    {
        public static Base64 GetBase64(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Base64.DefinitionId);
            return stereotype != null ? new Base64(stereotype) : null;
        }


        public static bool HasBase64(this AttributeModel model)
        {
            return model.HasStereotype(Base64.DefinitionId);
        }

        public static bool TryGetBase64(this AttributeModel model, out Base64 stereotype)
        {
            if (!HasBase64(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Base64(model.GetStereotype(Base64.DefinitionId));
            return true;
        }

        public static Compare GetCompare(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Compare.DefinitionId);
            return stereotype != null ? new Compare(stereotype) : null;
        }


        public static bool HasCompare(this AttributeModel model)
        {
            return model.HasStereotype(Compare.DefinitionId);
        }

        public static bool TryGetCompare(this AttributeModel model, out Compare stereotype)
        {
            if (!HasCompare(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Compare(model.GetStereotype(Compare.DefinitionId));
            return true;
        }

        public static Email GetEmail(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Email.DefinitionId);
            return stereotype != null ? new Email(stereotype) : null;
        }


        public static bool HasEmail(this AttributeModel model)
        {
            return model.HasStereotype(Email.DefinitionId);
        }

        public static bool TryGetEmail(this AttributeModel model, out Email stereotype)
        {
            if (!HasEmail(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Email(model.GetStereotype(Email.DefinitionId));
            return true;
        }

        public static MaxValue GetMaxValue(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(MaxValue.DefinitionId);
            return stereotype != null ? new MaxValue(stereotype) : null;
        }


        public static bool HasMaxValue(this AttributeModel model)
        {
            return model.HasStereotype(MaxValue.DefinitionId);
        }

        public static bool TryGetMaxValue(this AttributeModel model, out MaxValue stereotype)
        {
            if (!HasMaxValue(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MaxValue(model.GetStereotype(MaxValue.DefinitionId));
            return true;
        }

        public static MinValue GetMinValue(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(MinValue.DefinitionId);
            return stereotype != null ? new MinValue(stereotype) : null;
        }


        public static bool HasMinValue(this AttributeModel model)
        {
            return model.HasStereotype(MinValue.DefinitionId);
        }

        public static bool TryGetMinValue(this AttributeModel model, out MinValue stereotype)
        {
            if (!HasMinValue(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new MinValue(model.GetStereotype(MinValue.DefinitionId));
            return true;
        }

        public static Range GetRange(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Range.DefinitionId);
            return stereotype != null ? new Range(stereotype) : null;
        }


        public static bool HasRange(this AttributeModel model)
        {
            return model.HasStereotype(Range.DefinitionId);
        }

        public static bool TryGetRange(this AttributeModel model, out Range stereotype)
        {
            if (!HasRange(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Range(model.GetStereotype(Range.DefinitionId));
            return true;
        }

        public static RegularExpression GetRegularExpression(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(RegularExpression.DefinitionId);
            return stereotype != null ? new RegularExpression(stereotype) : null;
        }


        public static bool HasRegularExpression(this AttributeModel model)
        {
            return model.HasStereotype(RegularExpression.DefinitionId);
        }

        public static bool TryGetRegularExpression(this AttributeModel model, out RegularExpression stereotype)
        {
            if (!HasRegularExpression(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new RegularExpression(model.GetStereotype(RegularExpression.DefinitionId));
            return true;
        }

        public static Required GetRequired(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Required.DefinitionId);
            return stereotype != null ? new Required(stereotype) : null;
        }


        public static bool HasRequired(this AttributeModel model)
        {
            return model.HasStereotype(Required.DefinitionId);
        }

        public static bool TryGetRequired(this AttributeModel model, out Required stereotype)
        {
            if (!HasRequired(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Required(model.GetStereotype(Required.DefinitionId));
            return true;
        }

        public static StringLength GetStringLength(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(StringLength.DefinitionId);
            return stereotype != null ? new StringLength(stereotype) : null;
        }


        public static bool HasStringLength(this AttributeModel model)
        {
            return model.HasStereotype(StringLength.DefinitionId);
        }

        public static bool TryGetStringLength(this AttributeModel model, out StringLength stereotype)
        {
            if (!HasStringLength(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new StringLength(model.GetStereotype(StringLength.DefinitionId));
            return true;
        }

        public static Url GetUrl(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(Url.DefinitionId);
            return stereotype != null ? new Url(stereotype) : null;
        }


        public static bool HasUrl(this AttributeModel model)
        {
            return model.HasStereotype(Url.DefinitionId);
        }

        public static bool TryGetUrl(this AttributeModel model, out Url stereotype)
        {
            if (!HasUrl(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new Url(model.GetStereotype(Url.DefinitionId));
            return true;
        }

        public class Base64
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "02308621-429c-4af4-9428-2ebb272e53fa";

            public Base64(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class Compare
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "8a9c8792-14dc-4bdf-8b1b-1801c7c6a9f5";

            public Compare(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public IElement CompareTo()
            {
                return _stereotype.GetProperty<IElement>("Compare To");
            }

        }

        public class Email
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "9fb8d1b1-39b3-4f16-88e0-34d24a4e9bf6";

            public Email(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class MaxValue
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "5ffee79d-e11e-4ae8-921e-a61e79624afa";

            public MaxValue(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? Value()
            {
                return _stereotype.GetProperty<int?>("Value");
            }

        }

        public class MinValue
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c34a4c4b-e3ad-4cf5-98a4-5e13b4eb21ec";

            public MinValue(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? Value()
            {
                return _stereotype.GetProperty<int?>("Value");
            }

        }

        public class Range
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "0e6009db-a169-452f-a053-f35871dae376";

            public Range(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? From()
            {
                return _stereotype.GetProperty<int?>("From");
            }

            public int? To()
            {
                return _stereotype.GetProperty<int?>("To");
            }

        }

        public class RegularExpression
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "3dd144bc-374b-4acd-841a-7323210df66d";

            public RegularExpression(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string Pattern()
            {
                return _stereotype.GetProperty<string>("Pattern");
            }

        }

        public class Required
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "14680476-e24a-490f-ba44-75eb8dc6fb46";

            public Required(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

        public class StringLength
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "c5f8489e-005f-4e39-bf5c-7ef6983d4cbc";

            public StringLength(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? MaxLength()
            {
                return _stereotype.GetProperty<int?>("Max Length");
            }

        }

        public class Url
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "1b2dc31a-599f-449b-9646-1a5313d23f91";

            public Url(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

        }

    }
}
