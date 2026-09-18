namespace Intent.Agent.Gate;

/// <summary>
/// A parsed semantic version. Comparison follows semver PRECEDENCE, not the version's own
/// string form - the trap this exists to avoid is that "1.0.0-pre.10" sorts ABOVE
/// "1.0.0-pre.9" (pre-release identifiers that are both numeric compare numerically), while a
/// naive string comparison would put "pre.10" first.
/// </summary>
public readonly record struct SemVer(int Major, int Minor, int Patch, string? PreRelease)
{
    public static bool TryParse(string value, out SemVer version)
    {
        version = default;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var dashIndex = value.IndexOf('-');
        var core = dashIndex >= 0 ? value[..dashIndex] : value;
        var preRelease = dashIndex >= 0 ? value[(dashIndex + 1)..] : null;

        var parts = core.Split('.');
        if (parts.Length != 3
            || !int.TryParse(parts[0], out var major)
            || !int.TryParse(parts[1], out var minor)
            || !int.TryParse(parts[2], out var patch))
        {
            return false;
        }

        version = new SemVer(major, minor, patch, string.IsNullOrEmpty(preRelease) ? null : preRelease);
        return true;
    }

    /// <summary>
    /// Semver precedence comparison. A pre-release sorts BELOW the same core version with no
    /// suffix (so "1.0.0-pre.9" -> "1.0.0" is a legal promotion, not a violation). Pre-release
    /// identifiers compare numerically when both sides of a dot-segment are numeric, and
    /// lexically otherwise - per the semver spec, not a plain string comparison.
    /// </summary>
    public static int ComparePrecedence(SemVer a, SemVer b)
    {
        var core = a.Major.CompareTo(b.Major);
        if (core != 0)
        {
            return core;
        }

        core = a.Minor.CompareTo(b.Minor);
        if (core != 0)
        {
            return core;
        }

        core = a.Patch.CompareTo(b.Patch);
        if (core != 0)
        {
            return core;
        }

        if (a.PreRelease is null && b.PreRelease is null)
        {
            return 0;
        }

        if (a.PreRelease is null)
        {
            return 1;
        }

        if (b.PreRelease is null)
        {
            return -1;
        }

        return ComparePreReleaseIdentifiers(a.PreRelease, b.PreRelease);
    }

    private static int ComparePreReleaseIdentifiers(string a, string b)
    {
        var aParts = a.Split('.');
        var bParts = b.Split('.');
        var length = Math.Min(aParts.Length, bParts.Length);

        for (var i = 0; i < length; i++)
        {
            var aIsNumeric = int.TryParse(aParts[i], out var aNum);
            var bIsNumeric = int.TryParse(bParts[i], out var bNum);

            int cmp;
            if (aIsNumeric && bIsNumeric)
            {
                cmp = aNum.CompareTo(bNum);
            }
            else if (aIsNumeric != bIsNumeric)
            {
                // A numeric identifier always has lower precedence than an alphanumeric one.
                cmp = aIsNumeric ? -1 : 1;
            }
            else
            {
                cmp = string.CompareOrdinal(aParts[i], bParts[i]);
            }

            if (cmp != 0)
            {
                return cmp;
            }
        }

        return aParts.Length.CompareTo(bParts.Length);
    }

    public override string ToString() => PreRelease is null ? $"{Major}.{Minor}.{Patch}" : $"{Major}.{Minor}.{Patch}-{PreRelease}";
}