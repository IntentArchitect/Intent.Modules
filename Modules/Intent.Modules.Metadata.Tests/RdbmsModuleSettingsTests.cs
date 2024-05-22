using Intent.Modules.Metadata.RDBMS.Settings;
using Xunit;

namespace Intent.Modules.Metadata.Tests;

public class RdbmsModuleSettingsTests
{
    [Theory]
    [InlineData("10,2")]
    [InlineData(" 10,2")]
    [InlineData("10,2 ")]
    [InlineData(" 10,2 ")]
    [InlineData("10, 2")]
    [InlineData("10 , 2")]
    [InlineData(" 10 , 2 ")]
    [InlineData("(10,2)")]
    [InlineData("(10, 2)")]
    [InlineData("(10 , 2)")]
    [InlineData(" (10,2) ")]
    [InlineData(" ( 10,2 ) ")]
    public void ValidDatabaseDecimalConstraints(string format)
    {
        var result = DecimalConstraints.TryParse(format, out var constraints);
        Assert.True(result);
        Assert.Equal(10, constraints.Precision);
        Assert.Equal(2, constraints.Scale);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("1")]
    [InlineData("1,2,3")]
    [InlineData("1a, 2")]
    [InlineData("1, 2b")]
    public void InvalidDatabaseDecimalConstraints(string? format)
    {
        var result = DecimalConstraints.TryParse(format, out var constraints);
        Assert.False(result);
        Assert.Null(constraints);
    }
}