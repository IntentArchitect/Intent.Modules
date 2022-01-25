namespace Intent.Modules.Metadata.RDBMS.Api.Indexes
{
    public class Index
    {
        public bool IsUnique { get; set; }
        public string Name { get; set; }
        public FilterOption FilterOption { get; set; }
        public string Filter { get; set; }
        public IndexColumn[] KeyColumns { get; set; }
        public IndexColumn[] IncludedColumns { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Name)} = '{Name}', {nameof(FilterOption)} = '{FilterOption}', {nameof(Filter)} = '{Filter}'";
        }
    }
}