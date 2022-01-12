using Intent.Modules.Common.CSharp.Templates;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Templates
{
    public class TemplateExtensionsTests
    {
        public class DescribeToCSharpIdentifier
        {
            [Theory]
            [InlineData("Shortcut (macOS)", "ShortcutMacOS")]
            [InlineData("4something", "_4something")]
            [InlineData("Start!@#$%^&**(){}[]:\"|;'\\<>?,./End", "StartSharpAndEnd")]
            [InlineData("@string", "@string")]
            [InlineData(".NET", "NET")]
            [InlineData(".1NET", "_1NET")]
            public void ItShouldDoItCorrectly(string input, string expected)
            {
                // Act
                var actual = input.ToCSharpIdentifier(CapitalizationBehaviour.AsIs);

                // Assert
                Assert.Equal(expected, actual);
            }
        }
    }
}
