namespace Intent.Modules.Metadata.RDBMS.Api.Indexes
{
    public class IndexColumn
    {
        public string Name { get; set; }
        public IndexColumnType Type { get; set; }
        public bool IsDerived { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Name)} = '{Name}', {nameof(Type)} = '{Type}', {nameof(IsDerived)} = '{IsDerived}'";
        }
    }
}