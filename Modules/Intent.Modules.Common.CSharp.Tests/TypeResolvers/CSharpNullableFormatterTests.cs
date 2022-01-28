using System.Collections.Generic;
using Intent.Engine;
using Intent.Metadata.Models;
using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.TypeResolution;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.TypeResolvers
{
    public class CSharpNullableFormatterTests
    {
        [Theory]
        [InlineData("IList<string>")]
        [InlineData("List<string>")]
        [InlineData("string")]
        public void ItShouldDoItCorrectly(string input)
        {
            // Arrange
            var project = Substitute.For<IOutputTarget>();
            project.Metadata.Returns(new Dictionary<string, object>
            {
                ["Language Version"] = "latest",
                ["Nullable Enabled"] = true
            });
            var sut = new CSharpNullableFormatter(new CSharpProject(project));

            var typeReference = Substitute.For<ITypeReference>();
            typeReference.IsNullable.Returns(true);

            // Act
            var result = sut.AsNullable(new ResolvedTypeInfo(input, false, typeReference, null));

            // Assert
            result.ShouldBe($"{input}?");
        }
    }
}
