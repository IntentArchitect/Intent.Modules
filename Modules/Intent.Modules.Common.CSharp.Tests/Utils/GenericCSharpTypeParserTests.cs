using Intent.Modules.Common.CSharp.Utils;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Utils;

public class GenericCSharpTypeParserTests
{
    [Theory]
    [InlineData("void")]
    [InlineData("string")]
    [InlineData("System.Uri")]
    [InlineData("MyNamespace.MyClass")]
    [InlineData("int[]")]
    [InlineData("int?")]
    [InlineData("List<string>")]
    [InlineData("System.Collections.Generic.List<System.Uri>")]
    [InlineData("System.Collections.Generic.List<MyNamespace.MyClass>")]
    [InlineData("System.Threading.Tasks.Task<Media.Api.Application.Common.Pagination.PagedResult<System.Collection.Generics.Dictionary<System.Guid, System.Collection.Generics.Dictionary<string, byte[]>>>>")]
    [InlineData("(string, int)")]
    [InlineData("(string Name, int Age)")]
    [InlineData("List<(string, int, bool)>")]
    [InlineData("(List<DateTime>, int)")]
    [InlineData("(List<DateTime?>?, int?)?")]
    [InlineData("(List<DateTime[]>[], int[])[]")]
    [InlineData("(decimal, Dictionary<string, (bool, List<DateTime>, int)>)")]
    [InlineData("SpecialType<,,>")]
    [InlineData("(string Endpoint, Dictionary<string, string> ResourceAttributes)")]
    public void NoTransformationParsingTest(string? fullTypeName)
    {
        var result = GenericCSharpTypeParser.Parse(fullTypeName, name => name);
        Assert.Equal(fullTypeName, result);
    }

    [Fact]
    public void NoTransformationParsingNullTest()
    {
        var result = GenericCSharpTypeParser.Parse(null, name => name);
        Assert.Equal("void", result);
    }
}