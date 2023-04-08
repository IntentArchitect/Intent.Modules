namespace Intent.Modules.Common.TypeResolution;

/// <summary>
/// When implemented, <see cref="TypeResolverBase.AddTypeSource(ITypeSource,string)"/> will call
/// all the methods of this interface.
/// </summary>
public interface ICanUseDefaultFormatters : ITypeSource
{
    /// <summary>
    /// Sets the default <see cref="ICollectionFormatter"/>.
    /// </summary>
    void SetDefaultCollectionFormatter(ICollectionFormatter collectionFormatter);
    /// <summary>
    /// Sets the default <see cref="INullableFormatter"/>.
    /// </summary>
    void SetDefaultNullableFormatter(INullableFormatter nullableFormatter);
}