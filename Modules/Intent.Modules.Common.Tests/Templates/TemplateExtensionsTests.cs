using Intent.Modules.Common.Templates;
using Shouldly;
using Xunit;
// ReSharper disable InvokeAsExtensionMethod

namespace Intent.Modules.Common.Tests.Templates
{
    public class TemplateExtensionsTests
    {
        public class DescribeToSnakeCase
        {
            [Theory]
            [InlineData("app1", "app1")]
            [InlineData("SomeApp1Something", "some_app1_something")]
            [InlineData("new_app_1", "new_app_1")]
            public void ItShouldWork(string input, string expected)
            {
                // Act
                var result = TemplateExtensions.ToSnakeCase(input);

                // Assert
                result.ShouldBe(expected);
            }
        }

        public class DescribePluralize
        {
            [Theory]
            [InlineData("Apples", "Apples", false)]
            [InlineData("Apples", "Apples", true)]
            [InlineData("Apples", "Apples", null)]
            [InlineData("Apple", "Apples", false)]
            [InlineData("Apple", "Apples", true)]
            [InlineData("Apple", "Apples", null)]
            [InlineData("People", "People", false)]
            [InlineData("People", "Peoples", true)]
            [InlineData("People", "Peoples", null)]
            [InlineData("Person", "People", false)]
            [InlineData("Person", "People", true)]
            [InlineData("Person", "People", null)]
            public void ItShouldWork(string input, string expected, bool? inputIsKnownToBeSingular)
            {
                // Act
                var result = inputIsKnownToBeSingular.HasValue
                    ? TemplateExtensions.Pluralize(input, inputIsKnownToBeSingular.Value)
                    : TemplateExtensions.Pluralize(input);

                // Assert
                result.ShouldBe(expected);
            }
        }

        public class DescribeSingularize
        {
            [Theory]
            [InlineData("Apple", "Apple", false)]
            [InlineData("Apple", "Apple", true)]
            [InlineData("Apple", "Apple", null)]
            [InlineData("Apples", "Apple", false)]
            [InlineData("Apples", "Apple", true)]
            [InlineData("Apples", "Apple", null)]
            [InlineData("Person", "Person", false)]
            [InlineData("Person", "Person", true)]
            [InlineData("Person", "Person", null)]
            [InlineData("People", "Person", false)]
            [InlineData("People", "Person", true)]
            [InlineData("People", "Person", null)]
            public void ItShouldWork(string input, string expected, bool? inputIsKnownToBePlural)
            {
                // Act
                var result = inputIsKnownToBePlural.HasValue
                    ? TemplateExtensions.Singularize(input, inputIsKnownToBePlural.Value)
                    : TemplateExtensions.Singularize(input);

                // Assert
                result.ShouldBe(expected);
            }
        }
    }
}
