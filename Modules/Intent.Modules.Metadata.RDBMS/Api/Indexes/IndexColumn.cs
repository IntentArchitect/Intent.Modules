using Intent.Metadata.Models;

namespace Intent.Modules.Metadata.RDBMS.Api.Indexes
{
    /// <summary>
    /// Details of an indexed column.
    /// </summary>
    public class IndexColumn
    {
        /// <summary>
        /// The name of the column.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The element to which the index column is mapped to in Intent Architect.
        /// </summary>
        public ICanBeReferencedType SourceType { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Name)} = '{Name}'";
        }
    }
}