using System.Diagnostics.CodeAnalysis;

namespace Intent.Modules.EntityFrameworkCore
{
    [SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
    internal static class Stereotypes
    {
        public static class EntityFrameworkCore
        {
            public static class EFMappingOptions
            {
                public const string Name = "EFMappingOptions";

                public static class Property
                {
                    public const string ColumnType = "ColumnType";
                }
            }

            public static class ConcurrencyToken
            {
                public const string Name = "ConcurrencyToken";
            }

            public static class RowVersion
            {
                public const string Name = "RowVersion";
            }
        }

        public static class Rdbms
        {
            public static class DefaultConstraint
            {
                public const string Name = "Default Constraint";

                public static class Property
                {
                    public const string Name = "Name";
                    public const string Value = "Value";
                }
            }

            public static class ForeignKey
            {
                public const string Name = "Foreign Key";

                public static class Property
                {
                    public const string ColumnName = "Column Name";
                }
            }

            public static class Index
            {
                public const string Name = "Index";

                public static class Property
                {
                    public const string UniqueKey = "UniqueKey";
                    public const string Order = "Order";
                    public const string IsUnique = "IsUnique";
                }
            }

            public static class Numeric
            {
                public const string Name = "Numeric";

                public static class Property
                {
                    public const string Precision = "Precision";
                    public const string Scale = "Scale";
                }
            }

            public static class PrimaryKey
            {
                public const string Name = "Primary Key";

                public static class Property
                {
                    public const string Identity = "Identity";
                }
            }

            public static class Table
            {
                public const string Name = "Table";

                public static class Property
                {
                    public const string Schema = "Schema";
                    public const string Name = "Name";
                }
            }

            public static class Text
            {
                public const string Name = "Text";

                public static class Property
                {
                    public const string MaxLength = "MaxLength";
                    public const string IsUnicode = "IsUnicode";
                }
            }
        }

        // Stereotype names which don't appear in either packages above:
        public static class Legacy
        {
            public static class StringOptions
            {
                public const string Name = "StringOptions";

                public static class Property
                {
                    public const string MaxLength = "MaxLength";
                }
            }

            public static class Numeric
            {
                public const string Name = "Numeric";

                public static class Property
                {
                    public const string DataType = "DataType";
                }
            }
        }
    }
}
