using System.Collections.Generic;
using System.Linq;
using Intent.Metadata.Models;
using Intent.Modelers.Domain.Api;

namespace Intent.Metadata.RDBMS.Api
{
    public static class ClassModelUniqueConstraintExtensions
    {
        /// <summary>
        /// Gets the unique constraints specified for this class in the Domain designer.
        /// </summary>
        /// <param name="class"></param>
        /// <returns></returns>
        public static IEnumerable<UniqueConstraint> GetUniqueConstraints(this ClassModel @class)
        {
            return @class.Attributes
                .Where(x => x.HasUniqueConstraint())
                .Select(x => (ConstraintName: x.GetUniqueConstraint().Name(), Field: (ICanBeReferencedType)x.InternalElement, FieldName: x.Name, ColumnName: x.GetColumn()?.Name()))
                .Concat(@class.AssociatedClasses
                    .Where(x => x.HasUniqueConstraint())
                    .Select(x => (ConstraintName: x.GetUniqueConstraint().Name(), Field: (ICanBeReferencedType)x, FieldName: x.Name, ColumnName: x.GetForeignKey()?.ColumnName()))
                )
                .GroupBy(x => x.ConstraintName)
                .Select(x => new UniqueConstraint(x.Key, x.Select(f => new UniqueConstraintField(f.FieldName, f.ColumnName, f.Field)).ToList()));
        }

        /// <summary>
        /// Represents a unique constraint.
        /// </summary>
        public class UniqueConstraint
        {
            public UniqueConstraint(string constraintName, IList<UniqueConstraintField> fields)
            {
                ConstraintName = constraintName;
                Fields = fields;
            }
            /// <summary>
            /// The name of the constraint. Will be null if it was not specified.
            /// </summary>
            public string ConstraintName { get; set; }
            /// <summary>
            /// The fields grouped by the <see cref="ConstraintName"/>
            /// </summary>
            public IList<UniqueConstraintField> Fields { get; set; }
        }

        /// <summary>
        /// Represents a field associated by a unique constraint.
        /// </summary>
        public class UniqueConstraintField
        {
            public UniqueConstraintField(string name, string columnName, ICanBeReferencedType element)
            {
                Name = name;
                ColumnName = columnName;
                Element = element;
            }
            /// <summary>
            /// Name of the Attribute or Association
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// The column name specified. Will be null unless explicitly specified by a Column or Foreign Key stereotype.
            /// </summary>
            public string ColumnName { get; set; }
            /// <summary>
            /// The underlying element type
            /// </summary>
            public ICanBeReferencedType Element { get; set; }
        }
    }
}