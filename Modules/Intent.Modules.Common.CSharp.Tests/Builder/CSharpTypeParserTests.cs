using Intent.Modules.Common.CSharp.Builder;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class CSharpTypeParserTests
{
    [Fact]
    public void TryParseStringForType_Void()
    {
        var type = CSharpTypeParser.Parse("void");
        Assert.Equal(new CSharpTypeVoid(), type);
    }
    
    [Fact]
    public void TryParseStringForType_PredefinedType()
    {
        var type = CSharpTypeParser.Parse("string");
        Assert.Equal(new CSharpTypeName("string"), type);
    }
    
    [Theory]
    [InlineData("System.Uri")]
    [InlineData("MyNamespace.MyClass")]
    public void TryParseStringForType_NamedType(string testTypeName)
    {
        var type = CSharpTypeParser.Parse(testTypeName);
        Assert.Equal(new CSharpTypeName(testTypeName), type);
    }

    [Fact]
    public void TryParseStringForType_TypeAsArray()
    {
        var type = CSharpTypeParser.Parse("int[]");
        Assert.Equal(new CSharpTypeArray(new CSharpTypeName("int")), type);
    }
    
    [Fact]
    public void TryParseStringForType_NullableType()
    {
        var type = CSharpTypeParser.Parse("int?");
        Assert.Equal(new CSharpTypeNullable(new CSharpTypeName("int")), type);
    }
    
    [Theory]
    [InlineData("List<string>", "List", "string")]
    [InlineData("System.Collections.Generic.List<System.Uri>", "System.Collections.Generic.List", "System.Uri")]
    [InlineData("System.Collections.Generic.List<MyNamespace.MyClass>", "System.Collections.Generic.List", "MyNamespace.MyClass")]
    public void TryParseStringForType_GenericList_PredefinedType(string testTypeName, string genericName, string elementName)
    {
        var type = CSharpTypeParser.Parse(testTypeName);
        Assert.Equal(new CSharpTypeGeneric(genericName, [new CSharpTypeName(elementName)]), type);
    }
    
    [Fact]
    public void TryParseStringForType_Tuple_PredefinedTypes()
    {
        var type = CSharpTypeParser.Parse("(string, int)");
        Assert.Equal(new CSharpTypeTuple([new CSharpTupleElement(new CSharpTypeName("string")), new CSharpTupleElement(new CSharpTypeName("int"))]), type);
    }
    
    [Fact]
    public void TryParseStringForType_Tuple_PredefinedTypesWithNames()
    {
        var type = CSharpTypeParser.Parse("(string Name, int Age)");
        Assert.Equal(new CSharpTypeTuple([new CSharpTupleElement(new CSharpTypeName("string"), "Name"), new CSharpTupleElement(new CSharpTypeName("int"), "Age")]), type);
    }
    
    [Fact]
    public void TryParseStringForType_GenericList_Tuple_PredefinedTypes()
    {
        var type = CSharpTypeParser.Parse("List<(string, int, bool)>");
        Assert.Equal(new CSharpTypeGeneric("List", [
            new CSharpTypeTuple([
                new CSharpTupleElement(new CSharpTypeName("string")), 
                new CSharpTupleElement(new CSharpTypeName("int")),
                new CSharpTupleElement(new CSharpTypeName("bool"))
            ])
        ]), type);
    }
    
    [Fact]
    public void TryParseStringForType_TupleWithStartingGeneric()
    {
        var type = CSharpTypeParser.Parse("(List<DateTime>, int)");
        Assert.Equal(new CSharpTypeTuple([
            new CSharpTupleElement(new CSharpTypeGeneric("List", [new CSharpTypeName("DateTime")])),
            new CSharpTupleElement(new CSharpTypeName("int"))
        ]), type);
    }
    
    [Fact]
    public void TryParseStringForType_TupleWithStartingGenericAsNullables()
    {
        var type = CSharpTypeParser.Parse("(List<DateTime?>?, int?)?");
        Assert.Equal(new CSharpTypeNullable(new CSharpTypeTuple([
            new CSharpTupleElement(new CSharpTypeNullable(new CSharpTypeGeneric("List", [new CSharpTypeNullable(new CSharpTypeName("DateTime"))]))),
            new CSharpTupleElement(new CSharpTypeNullable(new CSharpTypeName("int")))
        ])), type);
    }
    
    [Fact]
    public void TryParseStringForType_TupleWithStartingGenericAsArrays()
    {
        var type = CSharpTypeParser.Parse("(List<DateTime[]>[], int[])[]");
        Assert.Equal(new CSharpTypeArray(new CSharpTypeTuple([
            new CSharpTupleElement(new CSharpTypeArray(new CSharpTypeGeneric("List", [new CSharpTypeArray(new CSharpTypeName("DateTime"))]))),
            new CSharpTupleElement(new CSharpTypeArray(new CSharpTypeName("int")))
        ])), type);
    }
    
    [Fact]
    public void TryParseStringForType_ComplexStructure()
    {
        var type = CSharpTypeParser.Parse("(decimal, Dictionary<string, (bool, List<DateTime>, int)>)");
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