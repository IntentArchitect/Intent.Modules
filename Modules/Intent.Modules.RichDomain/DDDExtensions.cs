using System;
using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel;
using Intent.SoftwareFactory.MetaModels.UMLModel;

namespace Intent.Modules.RichDomain
{
    public static class DDDExtensions
    {
        public static string TableName(this Class obj)
        {
            return obj.Stereotypes.GetTagValue("SchemaInfo", "TableName", obj.Name);
        }

        public static bool IsSubClass(this Class obj)
        {
            return obj.ParentClass != null;
        }

        public static string IdentifierType(this Class obj)
        {
            return StringExtensions.ToPascalCase(obj.Name) + "Id";
        }

        public static bool IsAggregateRoot(this Class obj)
        {
            return obj.Stereotypes.Any((s) => s.Name == "AggregateRoot");
        }

        public static bool IsEntity(this Class obj)
        {
            return obj.Stereotypes.Any((s) => s.Name == "AggregateRoot" || s.Name == "Entity");
        }

        public static bool IsValueObject(this Class obj)
        {
            return obj.Stereotypes.Any((s) => s.Name == "ValueObject");
        }

        public static string BoundedContext(this Class obj)
        {
            return obj.Package.Stereotypes.GetTagValue("BoundedContext", "Name", obj.Package.Name);
        }

        public static string BoundedContext(this EnumDefinition obj)
        {
            return obj.Package.Stereotypes.GetTagValue("BoundedContext", "Name", obj.Package.Name);
        }

        public static string FullName(this Class obj)
        {
            return string.Format("{0}.{1}", obj.BoundedContext(), obj.Name);
        }

        public static bool IsView(this Class obj)
        {
            return obj.Stereotypes.GetTagValue("SchemaInfo", "IsView", false);
        }

        public static IEnumerable<string> PrimaryKeys(this Class obj)
        {
            var stereoType = obj.Stereotypes.FirstOrDefault((s) => s.Name == "PrimaryKey");
            if (stereoType != null)
            {
                List<string> pk = new List<string>();
                string fa = null;
                string sa = null;
                string ta = null;
                foreach (var kvp in stereoType.Tags)
                {
                    switch (kvp.Key.ToLower())
                    {
                        case "firstattribute":
                            if (!string.IsNullOrWhiteSpace(kvp.Value))
                            {
                                fa = StringExtensions.ToPascalCase(kvp.Value);
                            }
                            break;
                        case "secondattribute":
                            if (!string.IsNullOrWhiteSpace(kvp.Value))
                            {
                                sa = StringExtensions.ToPascalCase(kvp.Value);
                            }
                            break;
                        case "thirdattribute":
                            if (!string.IsNullOrWhiteSpace(kvp.Value))
                            {
                                ta = StringExtensions.ToPascalCase(kvp.Value);
                            }
                            break;
                    }
                }
                if (!string.IsNullOrWhiteSpace(fa))
                {
                    pk.Add(fa);
                }
                if (!string.IsNullOrWhiteSpace(sa))
                {
                    pk.Add(sa);
                }
                if (!string.IsNullOrWhiteSpace(ta))
                {
                    pk.Add(ta);
                }
                return pk;
            }

            if (obj.IsEntity())
            {
                return new List<string>() { obj.Name + "Id" };
            }

            throw new Exception($"No primary key could be found for entity [{ obj.Name }]. Either stereotype the class as an Entity or indicate the primary key on the class' attributes.");
        }

        public static T GetTagValue<T>(this IEnumerable<Stereotype> stereotypes, string stereoTypeName, string tagName, T defaultValue)
        {
            foreach (var s in stereotypes)
            {
                if (s.Name == stereoTypeName)
                {
                    foreach (var tag in s.Tags)
                    {
                        if (tag.Key == tagName)
                        {
                            if (!string.IsNullOrWhiteSpace(tag.Value))
                            {
                                return (T)Convert.ChangeType(tag.Value, typeof(T));
                            }
                        }
                    }
                }
            }
            return defaultValue;
        }

        public static T GetTagValue<T>(this IEnumerable<Stereotype> stereotypes, string stereoTypeName, string tagName)
        {
            foreach (var s in stereotypes)
            {
                if (s.Name == stereoTypeName)
                {
                    foreach (var tag in s.Tags)
                    {
                        if (tag.Key == tagName)
                        {
                            if (!string.IsNullOrWhiteSpace(tag.Value))
                            {
                                if (Nullable.GetUnderlyingType(typeof(T)) != null)
                                {
                                    return (T)Convert.ChangeType(tag.Value, Nullable.GetUnderlyingType(typeof(T)));
                                }
                                return (T)Convert.ChangeType(tag.Value, typeof(T));
                            }
                        }
                    }
                }
            }
            return default(T);
        }


        public static bool IsQueryBehaviour(this Operation operation)
        {
            return operation.Stereotypes.Any((s) => s.Name == "QueryOperation");
        }

        public static bool IsCommandBehaviour(this Operation operation)
        {
            return operation.Stereotypes.Any((s) => s.Name == "CommandOperation");
        }

        public static string DomainType(this OperationParamter operationParameter)
        {
            return ConvertType(operationParameter.Type, operationParameter.IsNullable, operationParameter.IsCollection, operationParameter.ModelType, operationParameter.EnumDefinition);
        }

        public static bool HasComplexDomainType(this UmlAttribute attribute)
        {
            return ComplexDomainType(attribute) != null;
        }

        public static string ComplexDomainType(this UmlAttribute attribute)
        {
            return attribute.Stereotypes.GetTagValue<string>("DomainType", "Type");
        }

        public static string DomainType(this UmlAttribute attribute)
        {
            return ConvertType(attribute.Type, !attribute.IsMandatory, false, attribute.ModelType, attribute.EnumDefinition);
        }

        public static string Name(this AssociationEnd associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Role))
            {
                var className = associationEnd.Class.Name;
                if (associationEnd.MaxMultiplicity == "*" || int.Parse(associationEnd.MaxMultiplicity) > 1)
                {
                    return className.EndsWith("y") ? className.Substring(0, className.Length - 1) + "ies" : string.Format("{0}s", className);
                }
                return associationEnd.Class.Name;
            }

            return associationEnd.Role;
        }

        public static string SingluarName(this AssociationEnd associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Role))
            {
                return associationEnd.Class.Name;
            }
            if (associationEnd.MaxMultiplicity == "*" || int.Parse(associationEnd.MaxMultiplicity) > 1)
            {
                if (associationEnd.Role.EndsWith("ies"))
                {
                    return associationEnd.Role.Substring(0, associationEnd.Role.Length - 3) + "y";
                }
                if (associationEnd.Role.EndsWith("s"))
                {
                    return associationEnd.Role.Substring(0, associationEnd.Role.Length - 1);
                }
            }
            return associationEnd.Role;
        }

        public static string IdentifierName(this AssociationEnd associationEnd)
        {
            if (string.IsNullOrEmpty(associationEnd.Role))
            {
                return associationEnd.Class.IdentifierType();
            }
            else
            {
                return StringExtensions.ToPascalCase(associationEnd.Role) + "Id";// associationEnd.Class.IdentifierType();
            }
        }

        public static string IdentifierType(this AssociationEnd associationEnd)
        {
            return associationEnd.Class.IdentifierType();
        }

        public static string Type(this AssociationEnd associationEnd)
        {
            return associationEnd.Type("", "", false);
        }

        public static string Type(this AssociationEnd associationEnd, string suffix)
        {
            return associationEnd.Type("", suffix, false);
        }

        public static string Type(this AssociationEnd associationEnd, string prefix, string suffix)
        {
            return associationEnd.Type(prefix, suffix, false);
        }


        public static string Type(this AssociationEnd associationEnd, string suffix, bool readOnly)
        {
            return associationEnd.Type("", suffix, readOnly);
        }

        public static string Type(this AssociationEnd associationEnd, string prefix, string suffix, bool readOnly)
        {
            if (associationEnd.IsCollection())
            {
                if (readOnly)
                {
                    return "IEnumerable<" + prefix + associationEnd.Class.Name + suffix + ">";
                }
                else
                {
                    return "ICollection<" + prefix + associationEnd.Class.Name + suffix + ">";
                }
            }
            return prefix + associationEnd.Class.Name + suffix;
        }

        public static string ConstructorType(this AssociationEnd associationEnd)
        {
            if (associationEnd.IsCollection())
                return "List<" + associationEnd.Class.Name + ">";

            return associationEnd.Class.Name;

        }

        public static bool IsCollection(this AssociationEnd associationEnd)
        {
            return associationEnd.Multiplicity == Multiplicity.Many;
        }

        public static bool IsMandatory(this AssociationEnd associationEnd)
        {
            return associationEnd.MinMultiplicity != "0";
        }

        private static string ConvertType(string type, bool isNullable, bool isCollection, ModelType modelType, EnumDefinition enumDefinition)
        {
            if (type == null)
                return null;

            string typeConverted;
            switch (modelType)
            {
                case ModelType.Class:
                    typeConverted = "I" + ConvertType(type, false);
                    break;
                case ModelType.Enum:
                    var @namespace = enumDefinition.Stereotypes.GetTagValue<string>("CommonType", "Namespace", null) ?? enumDefinition.Stereotypes.GetTagValue<string>("C#", "Namespace", null);
                    type = @namespace != null ? @namespace + "." + type : type;
                    if (isCollection)
                    {
                        return "IEnumerable<" + type + ">";
                    }
                    var nullableString = (isNullable) ? "?" : "";
                    return type + nullableString;
                default:
                    typeConverted = ConvertType(type, isNullable);
                    break;
            }

            if (isCollection)
                typeConverted = "IEnumerable<" + typeConverted + ">";

            return typeConverted;
        }

        private static string ConvertType(string type, bool isNullable)
        {
            var nullableString = (isNullable) ? "?" : "";
            switch (type)
            {
                case "string":
                    return "System.String";
                case "date":
                    return "System.DateTime" + nullableString;
                case "datetime":
                    return "System.DateTime" + nullableString;
                case "guid":
                    return "System.Guid" + nullableString;
                case "int":
                    return "System.Int32" + nullableString;
                case "decimal":
                    return "System.Decimal" + nullableString;
                case "bool":
                case "boolean":
                    return "System.Boolean" + nullableString;
                case "binary":
                    return "Byte[]";
                default:
                    return type;
            }
        }
    }
}
