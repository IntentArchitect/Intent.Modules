using System;
using System.Collections.Generic;
using System.Text;
using Intent.MetaModel.UMLModel;
using Intent.Packages.RichDomain.Templates.EntityState;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Packages.RichDomain.Decorators
{
    public class EnforceConstraintsEntityStateDecorator : IDomainEntityStateDecorator
    {
        public const string Identifier = "Intent.RichDomain.Decorator.EnforceConstraints";

        public IEnumerable<string> DeclareUsings()
        {
            return new[]
            {
                "using Intent.Framework.Domain;",
            };
        }

        public string ClassAnnotations(Class @class)
        {
            return null;
        }

        public string ConstructorWithOrmLoadingParameter(Class @class)
        {
            return null;
        }

        public string PropertyFieldAnnotations(UmlAttribute attribute)
        {
            return null;
        }

        public string PropertyAnnotations(UmlAttribute attribute)
        {
            return null;
        }

        public string PropertySetterBefore(UmlAttribute attribute)
        {
            var sb = new StringBuilder();
            sb.Append(@"                if (!OrmLoading)
                {");
            foreach (var validation in GetSetPropertyValidations(attribute))
            {
                sb.Append($@"
                    if ({validation.Key})
                        throw new DomainViolationException(this, ""{validation.Value}"");");
            }
            sb.Append(@"
                }");
            return sb.ToString();
        }

        public string PropertySetterAfter(UmlAttribute attribute)
        {
            return null;
        }

        public string[] PublicProperties(Class @class)
        {
            return new string[0];
        }

        public Dictionary<string, string> GetSetPropertyValidations(UmlAttribute attribute)
        {
            var result = new Dictionary<string, string>();
            if (attribute.Type == "string" && attribute.IsMandatory)
            {
                result.Add("string.IsNullOrEmpty(value)", attribute.Name + " is mandatory.");
            }

            var minLength = attribute.Stereotypes.GetTagValue<int?>("Text", "MinLength");
            if (minLength != null && minLength > 0)
            {
                result.Add("!string.IsNullOrEmpty(value) && value.Length < " + minLength, attribute.Name + " cannot be less than " + minLength + " characters.");
            }

            var maxLength = attribute.Stereotypes.GetTagValue<int?>("Text", "MaxLength");
            if (maxLength != null)
            {
                result.Add("!string.IsNullOrEmpty(value) && value.Length > " + maxLength, attribute.Name + " cannot be more than " + maxLength + " characters.");
            }

            var minValue = attribute.Stereotypes.GetTagValue<int?>("Numeric", "MinValue");
            if (minValue != null)
            {
                result.Add("value < " + minValue, attribute.Name + " cannot be less than " + minValue + ".");
            }

            var maxValue = attribute.Stereotypes.GetTagValue<int?>("Numeric", "MaxValue");
            if (maxValue != null)
            {
                result.Add("value > " + maxValue, attribute.Name + " cannot be more than " + maxValue + ".");
            }

            var maxDecimalPlaces = attribute.Stereotypes.GetTagValue<int?>("Numeric", "MaxDecimalPlaces");
            if (maxDecimalPlaces != null)
            {
                result.Add(string.Format("(value * {0}) != Math.Floor(value * {0})", Math.Pow(10, maxDecimalPlaces.Value)), attribute.Name + " cannot have more than " + maxDecimalPlaces + " decimal places.");
            }

            return result;
        }
    }
}