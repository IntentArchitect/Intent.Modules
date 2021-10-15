namespace Intent.Modules.Common.TypeResolution
{
    /// <summary>
    /// Does not alter the name of the resolved type.
    /// </summary>
    public class DefaultNullableFormatter : INullableFormatter
    {
        public string AsNullable(IResolvedTypeInfo typeInfo)
        {
            return typeInfo.Name;
        }
    }
}