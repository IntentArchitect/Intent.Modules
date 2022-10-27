using Intent.Modules.Common.Java.Templates;
using Shouldly;

namespace Intent.Modules.Common.Java.Tests.Templates
{
    public class JavaIdentifierExtensionMethodsTests
    {
        public class DescribeToJavaIdentifier
        {
            [Theory]
            [InlineData("value", "Value")]
            public void ItShouldMakeFirstLetterUpper(string input, string expected)
            {
                // Act
                var result = input.ToJavaIdentifier(CapitalizationBehaviour.MakeFirstLetterUpper);

                // Assert
                result.ShouldBe(expected);
            }

            [Theory]
            [InlineData("Value", "value")]
            public void ItShouldMakeFirstLetterLower(string input, string expected)
            {
                // Act
                var result = input.ToJavaIdentifier(CapitalizationBehaviour.MakeFirstLetterLower);

                // Assert
                result.ShouldBe(expected);
            }

            [Theory]
            [InlineData("Value", "Value")]
            [InlineData("value", "value")]
            public void ItShouldKeepFirstLetterCasingAsIs(string input, string expected)
            {
                // Act
                var result = input.ToJavaIdentifier(CapitalizationBehaviour.AsIs);

                // Assert
                result.ShouldBe(expected);
            }

            [Theory]
            [InlineData("*true", "_true")]
            [InlineData("var", "_var")]
            [InlineData("final", "_final")]
            [InlineData("class", "_class")]
            public void ItShouldPrefixJavaKeywordsWithUnderscore(string input, string expected)
            {
                // Act
                var result = JavaIdentifierExtensionMethods.ToJavaIdentifier(input);

                // Assert
                result.ShouldBe(expected);
            }

            [Theory]
            [InlineData("1part", "_1part")]
            public void ItShouldPrefixInvalidStartingCharactersWithUnderscore(string input, string expected)
            {
                // Act
                var result = JavaIdentifierExtensionMethods.ToJavaIdentifier(input);

                // Assert
                result.ShouldBe(expected);
            }

            [Theory]
            [InlineData("part1[part2", "part1Part2")]
            public void ItShouldMakeNewWords(string input, string expected)
            {
                // Act
                var result = JavaIdentifierExtensionMethods.ToJavaIdentifier(input);

                // Assert
                result.ShouldBe(expected);
            }

            [Theory]
            [InlineData("part1-part2", "part1_part2")]
            public void ItShouldReplaceHyphensWithUnderscores(string input, string expected)
            {
                // Act
                var result = JavaIdentifierExtensionMethods.ToJavaIdentifier(input);

                // Assert
                result.ShouldBe(expected);
            }

            [Theory]
            [InlineData("$identifier", "$identifier")]
            [InlineData("identifier$part2", "identifier$part2")]
            [InlineData("£identifier", "£identifier")]
            public void ItShouldAllowCertainSpecialCharacters(string input, string expected)
            {
                // Act
                var result = JavaIdentifierExtensionMethods.ToJavaIdentifier(input);

                // Assert
                result.ShouldBe(expected);
            }
        }
    }
}
