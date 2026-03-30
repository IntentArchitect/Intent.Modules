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

        public static NumericLimits GetNumericLimits(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(NumericLimits.DefinitionId);
            return stereotype != null ? new NumericLimits(stereotype) : null;
        }


        public static bool HasNumericLimits(this AttributeModel model)
        {
            return model.HasStereotype(NumericLimits.DefinitionId);
        }

        public static bool TryGetNumericLimits(this AttributeModel model, out NumericLimits stereotype)
        {
            if (!HasNumericLimits(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new NumericLimits(model.GetStereotype(NumericLimits.DefinitionId));
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

        public static TextLimits GetTextLimits(this AttributeModel model)
        {
            var stereotype = model.GetStereotype(TextLimits.DefinitionId);
            return stereotype != null ? new TextLimits(stereotype) : null;
        }


        public static bool HasTextLimits(this AttributeModel model)
        {
            return model.HasStereotype(TextLimits.DefinitionId);
        }

        public static bool TryGetTextLimits(this AttributeModel model, out TextLimits stereotype)
        {
            if (!HasTextLimits(model))
            {
                stereotype = null;
                return false;
            }

            stereotype = new TextLimits(model.GetStereotype(TextLimits.DefinitionId));
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

        public class NumericLimits
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "cb14e47d-672c-4244-8950-7c4ebf8cf8ed";

            public NumericLimits(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string MinValue()
            {
                return _stereotype.GetProperty<string>("Min Value");
            }

            public string MaxValue()
            {
                return _stereotype.GetProperty<string>("Max Value");
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

        public class TextLimits
        {
            private IStereotype _stereotype;
            public const string DefinitionId = "13649b19-4dfe-43ec-967f-0b85a5801dd6";

            public TextLimits(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? MinLength()
            {
                return _stereotype.GetProperty<int?>("Min Length");
            }

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
