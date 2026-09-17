using Intent.Agent.Gate;
using Xunit;

namespace AgentGate.Tests;

/// <summary>
/// Precedence comparison - the semver traps this module was built around. See the plan's
/// "semver-traps" callout: numeric pre-release identifiers compare numerically, not lexically, and
/// a pre-release always sorts below the same core version with no suffix.
/// </summary>
public class SemVerTests
{
    [Theory]
    [InlineData("1.0.0-pre.9", "1.0.0-pre.10", -1)] // numeric identifier comparison, not lexical ("10" < "9" lexically)
    [InlineData("1.0.0-pre.10", "1.0.0-pre.9", 1)]
    [InlineData("1.0.0-pre.9", "1.0.0", -1)] // pre-release sorts below the same core version with no suffix
    [InlineData("1.0.0", "1.0.0-pre.9", 1)]
    [InlineData("1.1.0", "1.0.3-pre.0", 1)] // core version wins outright regardless of suffix
    [InlineData("1.0.3-pre.0", "1.1.0", -1)]
    [InlineData("1.0.0-pre.9", "1.0.0-pre.9", 0)]
    [InlineData("1.0.0", "1.0.1", -1)]
    public void ComparePrecedence_orders_by_semver_rules_not_string_comparison(string a, string b, int expectedSign)
    {
        Assert.True(SemVer.TryParse(a, out var versionA));
        Assert.True(SemVer.TryParse(b, out var versionB));

        var result = SemVer.ComparePrecedence(versionA, versionB);

        Assert.Equal(expectedSign, Math.Sign(result));
    }

    [Theory]
    [InlineData("1.0.0")]
    [InlineData("1.0.0-pre.0")]
    [InlineData("10.20.30-rc.1")]
    public void TryParse_accepts_valid_versions(string value)
    {
        Assert.True(SemVer.TryParse(value, out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-version")]
    [InlineData("1.0")]
    [InlineData("1.0.0.0")]
    public void TryParse_rejects_invalid_versions(string value)
    {
        Assert.False(SemVer.TryParse(value, out _));
    }
}
