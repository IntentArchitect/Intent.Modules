using Intent.Modules.Common.CSharp.TypeResolvers;
using Intent.Modules.Common.CSharp.VisualStudio;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.TypeResolvers
{
    public class CSharpCollectionFormatterTests
    {
        [Fact]
        public void ItShouldNotThrow()
        {
            // Arrange
            var project = Substitute.For<ICSharpProject>();
            project.IsNullableAwareContext().Returns(true);
            var nullableFormatter = CSharpNullableFormatter.Create(project);
            var formatter = CSharpCollectionFormatter.Create("{0}");

            var typeInfo = CSharpResolvedTypeInfo.Create(
                name: "TypeName",
                @namespace: "Namespace",
                isPrimitive: default,
                isNullable: true,
                isCollection: default,
                typeReference: default,
                template: default,
                nullableFormatter: nullableFormatter,
                genericTypeParameters: default);

            // Act
            var exception = Record.Exception(() => formatter.ApplyTo(typeInfo));

            // Assert
            exception.ShouldBeNull();
        }
    }
}

