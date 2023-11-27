using System;
using System.Globalization;

namespace Intent.Modules.Common.CSharp.VisualStudio;

/// <summary>
/// Represents a C# language version which is composed of a minor and major version and is
/// <see cref="IComparable"/> and <see cref="IEquatable{T}"/>.
/// </summary>
public struct MajorMinorVersion : IComparable<MajorMinorVersion>, IComparable, IEquatable<MajorMinorVersion>
{
    /// <summary>
    /// Creates a new instance of <see cref="MajorMinorVersion"/>.
    /// </summary>
    public static MajorMinorVersion Create(int major, int minor)
    {
        return new MajorMinorVersion
        {
            Major = major,
            Minor = minor
        };
    }

    /// <summary>
    /// Tries to parse the provided <paramref name="version"/> as a <see cref="MajorMinorVersion"/>.
    /// </summary>
    /// <returns><see langword="true"/> if the version could be parsed.</returns>
    public static bool TryParse(string version, out MajorMinorVersion parsedVersion)
    {
        parsedVersion = default;

        var split = version.Split('.');
        switch (split.Length)
        {
            case 1:
                {
                    if (int.TryParse(split[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var major))
                    {
                        parsedVersion = Create(major, 0);
                        return true;
                    }

                    return false;
                }
            case 2:
                {
                    if (int.TryParse(split[0], NumberStyles.Any, CultureInfo.InvariantCulture, out var major) &&
                        int.TryParse(split[1], NumberStyles.Any, CultureInfo.InvariantCulture, out var minor))
                    {
                        parsedVersion = Create(major, minor);
                        return true;
                    }

                    return false;
                }
            default:
                return false;
        }
    }

    /// <summary>
    /// Parse the provided <paramref name="version"/> as a <see cref="MajorMinorVersion"/> or
    /// throw an exception on failure.
    /// </summary>
    public static MajorMinorVersion Parse(string version)
    {
        if (TryParse(version, out var parsed))
        {
            return parsed;
        }

        throw new Exception($"Could not parse \"{version}\"");
    }

    /// <summary>
    /// The major version.
    /// </summary>
    public int Major { get; private set; }

    /// <summary>
    /// The minor version.
    /// </summary>
    public int Minor { get; private set; }

    /// <inheritdoc />
    public bool Equals(MajorMinorVersion other)
    {
        return Major == other.Major && Minor == other.Minor;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj is MajorMinorVersion other && Equals(other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Major, Minor);
    }

    /// <summary>
    /// Equal to operator.
    /// </summary>
    public static bool operator ==(MajorMinorVersion left, MajorMinorVersion right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Not equal to operator.
    /// </summary>
    public static bool operator !=(MajorMinorVersion left, MajorMinorVersion right)
    {
        return !left.Equals(right);
    }

    /// <inheritdoc />
    public int CompareTo(MajorMinorVersion other)
    {
        var majorComparison = Major.CompareTo(other.Major);
        if (majorComparison != 0) return majorComparison;
        return Minor.CompareTo(other.Minor);
    }

    /// <inheritdoc />
    public int CompareTo(object obj)
    {
        if (ReferenceEquals(null, obj)) return 1;
        return obj is MajorMinorVersion other ? CompareTo(other) : throw new ArgumentException($"Object must be of type {nameof(MajorMinorVersion)}");
    }

    /// <summary>
    /// Less than operator.
    /// </summary>
    public static bool operator <(MajorMinorVersion left, MajorMinorVersion right)
    {
        return left.CompareTo(right) < 0;
    }

    /// <summary>
    /// Greater than operator.
    /// </summary>
    public static bool operator >(MajorMinorVersion left, MajorMinorVersion right)
    {
        return left.CompareTo(right) > 0;
    }

    /// <summary>
    /// Less than or equal to operator.
    /// </summary>
    public static bool operator <=(MajorMinorVersion left, MajorMinorVersion right)
    {
        return left.CompareTo(right) <= 0;
    }

    /// <summary>
    /// Greater than or equal to operator.
    /// </summary>
    public static bool operator >=(MajorMinorVersion left, MajorMinorVersion right)
    {
        return left.CompareTo(right) >= 0;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Major:D}.{Minor:D}";
    }

    public void Deconstruct(out int major, out int minor)
    {
        major = Major;
        minor = Minor;
    }
}