using Intent.Modules.Common.CSharp.Builder;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class CSharpTypeParserTests
{
    [Fact]
    public void StringToType_PredefinedType()
    {
        Assert.True(CSharpTypeParser.TryParse("string", out var type));
        Assert.Equal(new CSharpTypeName("string"), type);
    }
    
    [Theory]
    [InlineData("System.Uri")]
    [InlineData("MyNamespace.MyClass")]
    public void StringToType_NamedType(string testTypeName)
    {
        Assert.True(CSharpTypeParser.TryParse(testTypeName, out var type));
        Assert.Equal(new CSharpTypeName(testTypeName), type);
    }
    
    [Theory]
    [InlineData("List<string>", "List", "string")]
    [InlineData("System.Collections.Generic.List<System.Uri>", "System.Collections.Generic.List", "System.Uri")]
    [InlineData("System.Collections.Generic.List<MyNamespace.MyClass>", "System.Collections.Generic.List", "MyNamespace.MyClass")]
    public void StringToType_GenericList_PredefinedType(string testTypeName, string genericName, string elementName)
    {
        Assert.True(CSharpTypeParser.TryParse(testTypeName, out var type));
        Assert.Equal(new CSharpTypeGeneric(genericName, [new CSharpTypeName(elementName)]), type);
    }
    
    [Fact]
    public void StringToType_Tuple_PredefinedTypes()
    {
        Assert.True(CSharpTypeParser.TryParse("(string, int)", out var type));
        Assert.Equal(new CSharpTypeTuple([new CSharpTupleElement(new CSharpTypeName("string")), new CSharpTupleElement(new CSharpTypeName("int"))]), type);
    }
    
    [Fact]
    public void StringToType_Tuple_PredefinedTypesWithNames()
    {
        Assert.True(CSharpTypeParser.TryParse("(string Name, int Age)", out var type));
        Assert.Equal(new CSharpTypeTuple([new CSharpTupleElement(new CSharpTypeName("string"), "Name"), new CSharpTupleElement(new CSharpTypeName("int"), "Age")]), type);
    }
    
    [Fact]
    public void StringToType_GenericList_Tuple_PredefinedTypes()
    {
        Assert.True(CSharpTypeParser.TryParse("List<(string, int, bool)>", out var type));
        Assert.Equal(new CSharpTypeGeneric("List", [
            new CSharpTypeTuple([
                new CSharpTupleElement(new CSharpTypeName("string")), 
                new CSharpTupleElement(new CSharpTypeName("int")),
                new CSharpTupleElement(new CSharpTypeName("bool"))
            ])
        ]), type);
    }
    
    [Fact]
    public void StringToType_TupleWithStartingGeneric()
    {
        Assert.True(CSharpTypeParser.TryParse("(List<DateTime>, int)", out var type));
        Assert.Equal(new CSharpTypeTuple([
            new CSharpTupleElement(new CSharpTypeGeneric("List", [new CSharpTypeName("DateTime")])),
            new CSharpTupleElement(new CSharpTypeName("int"))
        ]), type);
    }
    
    [Fact]
    public void StringToType_ComplexStructure()
    {
        Assert.True(CSharpTypeParser.TryParse("(decimal, Dictionary<string, (bool, List<DateTime>, int)>)", out var type));
        Assert.Equal(new CSharpTypeTuple([
                new CSharpTupleElement(new CSharpTypeName("decimal")),
                new CSharpTupleElement(new CSharpTypeGeneric("Dictionary", [
                    new CSharpTypeName("string"),
                    new CSharpTypeTuple([
                        new CSharpTupleElement(new CSharpTypeName("bool")),
                        new CSharpTupleElement(new CSharpTypeGeneric("List", [new CSharpTypeName("DateTime")])),
                        new CSharpTupleElement(new CSharpTypeName("int"))
                    ])
                ]))
            ]), type);
    }
}