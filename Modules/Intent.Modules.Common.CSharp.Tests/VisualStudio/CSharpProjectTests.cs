using System.Collections.Generic;
using Intent.Engine;
using Intent.Modules.Common.CSharp.VisualStudio;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.VisualStudio
{
    public class CSharpProjectTests
    {
        public class DescribeIsNullableAwareContext
        {
            [Theory]
            [InlineData(true, "net5.0", "default", true)]
            [InlineData(true, "net6.0", "default", true)]
            [InlineData(true, "net7.0", "default", true)]
            [InlineData(true, "netcoreapp1.0", "default", false)]
            [InlineData(true, "netcoreapp1.1", "default", false)]
            [InlineData(true, "netcoreapp2.0", "default", false)]
            [InlineData(true, "netcoreapp2.1", "default", false)]
            [InlineData(true, "netcoreapp2.2", "default", false)]
            [InlineData(true, "netcoreapp3.0", "default", true)]
            [InlineData(true, "netcoreapp3.1", "default", true)]
            [InlineData(true, "net45", "default", false)]
            [InlineData(true, "net451", "default", false)]
            [InlineData(true, "net452", "default", false)]
            [InlineData(true, "net46", "default", false)]
            [InlineData(true, "net461", "default", false)]
            [InlineData(true, "net462", "default", false)]
            [InlineData(true, "net47", "default", false)]
            [InlineData(true, "net471", "default", false)]
            [InlineData(true, "net472", "default", false)]
            [InlineData(true, "net48", "default", false)]
            [InlineData(true, "netstandard1.0", "default", false)]
            [InlineData(true, "netstandard1.1", "default", false)]
            [InlineData(true, "netstandard1.2", "default", false)]
            [InlineData(true, "netstandard1.3", "default", false)]
            [InlineData(true, "netstandard1.4", "default", false)]
            [InlineData(true, "netstandard1.5", "default", false)]
            [InlineData(true, "netstandard1.6", "default", false)]
            [InlineData(true, "netstandard2.0", "default", false)]
            [InlineData(true, "netstandard2.1", "default", true)]
            [InlineData(true, null, "latest", true)]
            [InlineData(true, null, "preview", true)]
            [InlineData(true, null, "8.1", true)]
            [InlineData(true, null, "8.0", true)]
            [InlineData(true, null, "7.9", false)]
            [InlineData(false, null, "latest", false)]
            [InlineData(false, null, "preview", false)]
            [InlineData(false, null, "8.1", false)]
            [InlineData(false, null, "8.0", false)]
            [InlineData(false, null, "7.9", false)]
            [InlineData(false, null, null, false)]
            public void ItShouldReturnTheCorrectResult(bool nullableEnabled, string targetFramework, string languageVersion, bool expected)
            {
                // Arrange
                var outputTarget = Substitute.For<IOutputTarget>();
                outputTarget.Metadata.Returns(new Dictionary<string, object>
                {
                    ["Nullable Enabled"] = nullableEnabled,
                    ["Language Version"] = languageVersion,
                    ["Target Frameworks"] = new[] { targetFramework }
                });
                var sut = new CSharpProject(outputTarget);

                // Act
                var result = sut.IsNullableAwareContext();

                // Assert
                result.ShouldBe(expected);
            }
        }
    }
}
