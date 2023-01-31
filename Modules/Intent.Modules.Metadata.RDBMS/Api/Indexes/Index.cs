using System.Collections.Generic;

namespace Intent.Modules.Metadata.RDBMS.Api.Indexes
{
    /// <summary>
    /// Details of an index.
    /// </summary>
    public class Index
    {
        /// <summary>
        /// Whether or not multiple rows are allowed to have the same value(s) for the index's column set.
        /// </summary>
        public bool IsUnique { get; set; }

        /// <summary>
        /// The name as captured for the index in the designer. Should be ignored when
        /// <see cref="UseDefaultName"/> is <see langword="true"/>.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Whether or not to use the default conventional name for the index.
        /// </summary>
        /// <remarks>
        /// By convention, indexes created in a relational database are named
        /// IX_&lt;type name&gt;_&lt;property name&gt;. For composite indexes, &lt;property name&gt;
        /// becomes an underscore separated list of property names.
        /// </remarks>
        public bool UseDefaultName { get; set; }

        /// <summary>
        /// Filtering option for the index.
        /// </summary>
        public FilterOption FilterOption { get; set; }

        /// <summary>
        /// The value to use when <see cref="FilterOption"/> is set to
        /// <see cref="FilterOption.Custom"/>.
        /// </summary>
        public string Filter { get; set; }

        /// <summary>
        /// One or more key columns for the index.
        /// </summary>
        public IndexColumn[] KeyColumns { get; set; }

        /// <summary>
        /// Some relational databases allow you to configure a set of columns which get included
        /// in the index, but aren't part of its "key". This can significantly improve query
        /// performance when all columns in the query are included in the index either as key or
        /// non-key columns, as the table itself doesn't need to be accessed.
        /// </summary>
        public IndexColumn[] IncludedColumns { get; set; }
        
        /// <summary>
        /// Some relational databases allow you to configure the "Fill Factor" for indexes as a percentage value.
        /// </summary>
        public int? FillFactor { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var propertyList = new List<string>
            {
                $"{nameof(Name)} = '{Name}'",
                $"{nameof(FilterOption)} = '{FilterOption}'",
                $"{nameof(Filter)} = '{Filter}'",
                $"{nameof(UseDefaultName)} = '{UseDefaultName}'"
            };

            if (FillFactor.HasValue)
            {
                propertyList.Add($"{nameof(FillFactor)} = '{FillFactor}'");
            }
            
            return string.Join(", ", propertyList);
        }
    }
}