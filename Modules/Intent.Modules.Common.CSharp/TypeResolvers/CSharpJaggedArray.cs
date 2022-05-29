namespace Intent.Modules.Common.CSharp.TypeResolvers;

/// <summary>
/// An array dimension for <see cref="CSharpResolvedTypeInfo"/>.
/// </summary>
/// <remarks>
/// See <see href="https://docs.microsoft.com/dotnet/csharp/programming-guide/arrays/multidimensional-arrays">Multidimensional Arrays</see>
/// and <see href="https://docs.microsoft.com/dotnet/csharp/programming-guide/arrays/jagged-arrays">Jagged Arrays</see>
/// for more information on arrays in C#.
/// </remarks>
public class CSharpJaggedArray
{
    /// <summary>
    /// Creates a new instance of <see cref="CSharpJaggedArray"/>.
    /// </summary>
    public CSharpJaggedArray(int dimensions = 0)
    {
        Dimensions = dimensions;
    }

    /// <summary>
    /// The number of dimensions of this jagged array.
    /// </summary>
    public int Dimensions { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{new string(',', Dimensions)}]";
    }
}