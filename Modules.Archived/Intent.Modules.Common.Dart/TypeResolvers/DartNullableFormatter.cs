using Intent.Modules.Common.TypeResolution;

namespace Intent.Modules.Common.Dart.TypeResolvers;

/// <summary>
/// Dart implementation of <see cref="INullableFormatter"/>.
/// </summary>
public class DartNullableFormatter : INullableFormatter
{
    private DartNullableFormatter()
    {
    }

    /// <summary>
    /// Singleton instance of <see cref="DartNullableFormatter" />.
    /// </summary>
    public static DartNullableFormatter Instance { get; } = new();

    /// <inheritdoc />
    public string AsNullable(IResolvedTypeInfo typeInfo, string type) => typeInfo.IsNullable ? $"{type}?" : type;
}