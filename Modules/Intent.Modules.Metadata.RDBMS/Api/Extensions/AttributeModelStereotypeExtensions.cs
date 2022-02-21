using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;
using Intent.Modules.Common;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementModelExtensions", Version = "1.0")]

namespace Intent.Metadata.RDBMS.Api
{

    public static class AttributeModelStereotypeExtensions
    {
        public static Column GetColumn(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Column");
            return stereotype != null ? new Column(stereotype) : null;
        }

        public static IReadOnlyCollection<Column> GetColumns(this AttributeModel model)
        {
            var stereotypes = model
                .GetStereotypes("Column")
                .Select(stereotype => new Column(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasColumn(this AttributeModel model)
        {
            return model.HasStereotype("Column");
        }

        public static ComputedValue GetComputedValue(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Computed Value");
            return stereotype != null ? new ComputedValue(stereotype) : null;
        }

        public static IReadOnlyCollection<ComputedValue> GetComputedValues(this AttributeModel model)
        {
            var stereotypes = model
                .GetStereotypes("Computed Value")
                .Select(stereotype => new ComputedValue(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasComputedValue(this AttributeModel model)
        {
            return model.HasStereotype("Computed Value");
        }

        public static DecimalConstraints GetDecimalConstraints(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Decimal Constraints");
            return stereotype != null ? new DecimalConstraints(stereotype) : null;
        }

        public static bool HasDecimalConstraints(this AttributeModel model)
        {
            return model.HasStereotype("Decimal Constraints");
        }

        public static DefaultConstraint GetDefaultConstraint(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Default Constraint");
            return stereotype != null ? new DefaultConstraint(stereotype) : null;
        }

        public static IReadOnlyCollection<DefaultConstraint> GetDefaultConstraints(this AttributeModel model)
        {
            var stereotypes = model
                .GetStereotypes("Default Constraint")
                .Select(stereotype => new DefaultConstraint(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasDefaultConstraint(this AttributeModel model)
        {
            return model.HasStereotype("Default Constraint");
        }

        public static Index GetIndex(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Index");
            return stereotype != null ? new Index(stereotype) : null;
        }

        public static IReadOnlyCollection<Index> GetIndices(this AttributeModel model)
        {
            var stereotypes = model
                .GetStereotypes("Index")
                .Select(stereotype => new Index(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasIndex(this AttributeModel model)
        {
            return model.HasStereotype("Index");
        }

        public static PrimaryKey GetPrimaryKey(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Primary Key");
            return stereotype != null ? new PrimaryKey(stereotype) : null;
        }

        public static IReadOnlyCollection<PrimaryKey> GetPrimaryKeys(this AttributeModel model)
        {
            var stereotypes = model
                .GetStereotypes("Primary Key")
                .Select(stereotype => new PrimaryKey(stereotype))
                .ToArray();

            return stereotypes;
        }

        public static bool HasPrimaryKey(this AttributeModel model)
        {
            return model.HasStereotype("Primary Key");
        }

        public static TextConstraints GetTextConstraints(this AttributeModel model)
        {
            var stereotype = model.GetStereotype("Text Constraints");
            return stereotype != null ? new TextConstraints(stereotype) : null;
        }

        public static bool HasTextConstraints(this AttributeModel model)
        {
            return model.HasStereotype("Text Constraints");
        }


        public class Column
        {
            private IStereotype _stereotype;

            public Column(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string Type()
            {
                return _stereotype.GetProperty<string>("Type");
            }

        }

        public class ComputedValue
        {
            private IStereotype _stereotype;

            public ComputedValue(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string SQL()
            {
                return _stereotype.GetProperty<string>("SQL");
            }

            public bool Stored()
            {
                return _stereotype.GetProperty<bool>("Stored");
            }

        }

        public class DecimalConstraints
        {
            private IStereotype _stereotype;

            public DecimalConstraints(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public int? Precision()
            {
                return _stereotype.GetProperty<int?>("Precision");
            }

            public int? Scale()
            {
                return _stereotype.GetProperty<int?>("Scale");
            }

        }

        public class DefaultConstraint
        {
            private IStereotype _stereotype;

            public DefaultConstraint(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string StereotypeName => _stereotype.Name;

            public string Name()
            {
                return _stereotype.GetProperty<string>("Name");
            }

            public string Value()
            {
                return _stereotype.GetProperty<string>("Value");
            }

        }

        public class Index
        {
            private IStereotype _stereotype;

            public Index(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public string UniqueKey()
            {
                return _stereotype.GetProperty<string>("UniqueKey");
            }

            public int? Order()
            {
                return _stereotype.GetProperty<int?>("Order");
            }

            public bool IsUnique()
            {
                return _stereotype.GetProperty<bool>("IsUnique");
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

            public bool Identity()
            {
                return _stereotype.GetProperty<bool>("Identity");
            }

        }

        public class TextConstraints
        {
            private IStereotype _stereotype;

            public TextConstraints(IStereotype stereotype)
            {
                _stereotype = stereotype;
            }

            public string Name => _stereotype.Name;

            public SQLDataTypeOptions SQLDataType()
            {
                return new SQLDataTypeOptions(_stereotype.GetProperty<string>("SQL Data Type"));
            }

            public int? MaxLength()
            {
                return _stereotype.GetProperty<int?>("MaxLength");
            }

            public bool IsUnicode()
            {
                return _stereotype.GetProperty<bool>("IsUnicode");
            }

            public class SQLDataTypeOptions
            {
                public readonly string Value;

                public SQLDataTypeOptions(string value)
                {
                    Value = value;
                }

                public SQLDataTypeOptionsEnum AsEnum()
                {
                    switch (Value)
                    {
                        case "VARCHAR":
                            return SQLDataTypeOptionsEnum.VARCHAR;
                        case "NVARCHAR":
                            return SQLDataTypeOptionsEnum.NVARCHAR;
                        case "TEXT":
                            return SQLDataTypeOptionsEnum.TEXT;
                        case "NTEXT":
                            return SQLDataTypeOptionsEnum.NTEXT;
                        case "DEFAULT":
                            return SQLDataTypeOptionsEnum.DEFAULT;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                public bool IsVARCHAR()
                {
                    return Value == "VARCHAR";
                }
                public bool IsNVARCHAR()
                {
                    return Value == "NVARCHAR";
                }
                public bool IsTEXT()
                {
                    return Value == "TEXT";
                }
                public bool IsNTEXT()
                {
                    return Value == "NTEXT";
                }
                public bool IsDEFAULT()
                {
                    return Value == "DEFAULT";
                }
            }

            public enum SQLDataTypeOptionsEnum
            {
                VARCHAR,
                NVARCHAR,
                TEXT,
                NTEXT,
                DEFAULT
            }
        }

    }
}