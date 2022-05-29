namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Does not alter the name of the resolved type.
    /// </summary>
    public class DefaultNullableFormatter : INullableFormatter
    {
        /// <summary>
        /// Obsolete. Use <see cref="Instance"/> instead.
        /// </summary>
        private DefaultNullableFormatter()
        {
        }

        /// <summary>
        /// Singleton instance of <see cref="DefaultNullableFormatter"/>.
        /// </summary>
        public static INullableFormatter Instance { get; } = new DefaultNullableFormatter();

        /// <inheritdoc />
        public string AsNullable(IResolvedTypeInfo typeInfo, string type)
        {
            return type;
        }
    }
}