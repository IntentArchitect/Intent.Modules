using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Typescript.ServiceAgent.AngularJs.Templates.ServiceProxy
{
    internal static class WebApiHelpers
    {
        public static bool UsesBodyContent(this OperationModel operation)
        {
            if (!operation.Parameters.Any())
            {
                return false;
            }

            var verb = GetHttpVerb(operation);
            if (verb != HttpVerb.POST && verb != HttpVerb.PUT)
            {
                return false;
            }

            return operation.Parameters.Any(IsFromBody);
        }


        public static bool RequiresPayloadObject(this OperationModel operation)
        {
            if (!operation.Parameters.Any())
            {
                return false;
            }

            var verb = GetHttpVerb(operation);
            if (verb != HttpVerb.POST && verb != HttpVerb.PUT)
            {
                return false;
            }

            return operation.Parameters.Count(IsFromBody) > 1;
        }

        private static readonly ISet<string> UrlEncodablePrimitives = new HashSet<string>
        {
            "bool",
            "Boolean",
            "System.Boolean",
            "byte",
            "Byte",
            "System.Byte",
            "sbyte",
            "SByte",
            "System.SByte",
            "char",
            "Char",
            "System.Char",
            "decimal",
            "Decimal",
            "System.Decimal",
            "double",
            "Double",
            "System.Double",
            "float",
            "Single",
            "System.Single",
            "int",
            "Int32",
            "System.Int32",
            "uint",
            "UInt32",
            "System.UInt32",
            "long",
            "Int64",
            "System.Int64",
            "ulong",
            "UInt64",
            "System.UInt64",
            "short",
            "Int16",
            "System.Int16",
            "ushort",
            "UInt16",
            "System.UInt16",
            "string",
            "String",
            "System.String",
            "guid",
            "Guid",
            "System.Guid",
            "date",
            "datetime",
            "DateTime",
            "System.DateTime",
        };

        public static bool IsFromBody(this ParameterModel parameter)
        {
            // NB: Order of conditional checks is important here
            return GetParameterBindingAttribute(parameter) == "[FromBody]" || !UrlEncodablePrimitives.Contains(parameter.Type.Element.Name);
        }

        //public static string MakeUrlEncodeable(this OperationModelParameterModel parameter)
        //{
        //    if (!UrlEncodablePrimitives.Contains(parameter.TypeReference.Name))
        //    {
        //        return parameter.Name;
        //    }

        //    return string.Format(UrlEncodablePrimitives[parameter.TypeReference.Name], parameter.Name);
        //}

        private static string GetParameterBindingAttribute(this ParameterModel operationParameterModel)
        {
            const string parameterBinding = "Parameter Binding";
            const string propertyType = "Type";
            const string propertyCustomType = "Custom Type";
            const string customValue = "Custom";

            if (!operationParameterModel.HasStereotype(parameterBinding))
            {
                return string.Empty;
            }

            var attributeName = operationParameterModel.GetStereotypeProperty<string>(parameterBinding, propertyType);
            if (!string.Equals(attributeName, customValue, StringComparison.OrdinalIgnoreCase))
            {
                return $"[{attributeName}]";
            }

            var customAttributeValue = operationParameterModel.GetStereotypeProperty<string>(parameterBinding, propertyCustomType);
            if (string.IsNullOrWhiteSpace(customAttributeValue))
            {
                throw new System.Exception("Parameter Binding was set to custom but no Custom attribute type was specified");
            }

            return $"[{customAttributeValue}]";
        }

        public static HttpVerb GetHttpVerb(this OperationModel operation)
        {
            var verb = operation.GetStereotypeProperty("Http", "Verb", "AUTO").ToUpper();
            if (verb != "AUTO")
            {
                return Enum.TryParse(verb, out HttpVerb verbEnum) ? verbEnum : HttpVerb.POST;
            }

            if (operation.ReturnType == null || operation.Parameters.Any(IsFromBody))
            {
                return HttpVerb.POST;
            }

            return HttpVerb.GET;
        }
    }
}
