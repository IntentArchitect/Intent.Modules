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
            [InlineData("Hello!@#$%^&**(){}[]:\"|;'\\<>?,./", "HelloSharpAnd")]
            [InlineData("@string", "@string")]
            public void ItShouldDoItCorrectly(string input, string expected)
            {
                // Act
                var actual = input.ToCSharpIdentifier();

                // Assert
                Assert.Equal(expected, actual);
            }
        }
    }
}
