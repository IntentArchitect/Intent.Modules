using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class CSharpTypeTests
{
    [Fact]
    public void Type_Void()
    {
        Assert.Equal("void", new CSharpTypeVoid().ToString());
    }

    [Fact]
    public void Type_String()
    {
        Assert.Equal("string", new CSharpTypeName("string").ToString());
    }

    [Fact]
    public void Type_Task()
    {
        Assert.Equal("Task", new CSharpTypeName("Task").ToString());
    }

    [Fact]
    public void Type_Tuple_WithNames()
    {
        Assert.Equal("(string Name, string Surname)", new CSharpTypeTuple([
            new CSharpTupleElement(new CSharpTypeName("string"), "Name"),
            new CSharpTupleElement(new CSharpTypeName("string"), "Surname")
        ]).ToString());
    }

    [Fact]
    public void Type_Tuple_WithoutNames()
    {
        Assert.Equal("(string, string)", new CSharpTypeTuple([
            new CSharpTupleElement(new CSharpTypeName("string")),
            new CSharpTupleElement(new CSharpTypeName("string"))
        ]).ToString());
    }

    [Fact]
    public void Type_List_String()
    {
        Assert.Equal("List<string>", new CSharpTypeGeneric("List", [new CSharpTypeName("string")]).ToString());
    }

    [Fact]
    public void Type_Task_Bool()
    {
        Assert.Equal("Task<bool>", new CSharpTypeGeneric("Task", [new CSharpTypeName("bool")]).ToString());
    }

    [Fact]
    public void Type_Task_Tuple()
    {
        Assert.Equal("Task<(bool, string)>", new CSharpTypeGeneric("Task", [
            new CSharpTypeTuple([
                new CSharpTupleElement(new CSharpTypeName("bool")),
                new CSharpTupleElement(new CSharpTypeName("string"))
            ])
        ]).ToString());
    }

    [Fact]
    public void Type_Task_List_Int()
    {
        Assert.Equal("Task<List<int>>", new CSharpTypeGeneric("Task", [
            new CSharpTypeGeneric("List", [new CSharpTypeName("int")])
        ]).ToString());
    }

    [Fact]
    public void Type_Task_List_Tuple()
    {
        Assert.Equal("Task<List<(bool, string)>>", new CSharpTypeGeneric("Task", [
            new CSharpTypeGeneric("List", [
                new CSharpTypeTuple([
                    new CSharpTupleElement(new CSharpTypeName("bool")),
                    new CSharpTupleElement(new CSharpTypeName("string"))
                ])
            ])
        ]).ToString());
    }

    [Fact]
    public void StringToType_PredefinedType()
    {
        Assert.True(CSharpType.TryParse("string", out var type));
        Assert.Equal(new CSharpTypeName("string"), type);
    }
    
    [Theory]
    [InlineData("System.Uri")]
    [InlineData("MyNamespace.MyClass")]
    public void StringToType_NamedType(string testTypeName)
    {
        Assert.True(CSharpType.TryParse(testTypeName, out var type));
        Assert.Equal(new CSharpTypeName(testTypeName), type);
    }
    
    [Theory]
    [InlineData("List<string>", "List", "string")]
    [InlineData("System.Collections.Generic.List<System.Uri>", "System.Collections.Generic.List", "System.Uri")]
    [InlineData("System.Collections.Generic.List<MyNamespace.MyClass>", "System.Collections.Generic.List", "MyNamespace.MyClass")]
    public void StringToType_GenericList_PredefinedType(string testTypeName, string genericName, string elementName)
    {
        Assert.True(CSharpType.TryParse(testTypeName, out var type));
        Assert.Equal(new CSharpTypeGeneric(genericName, [new CSharpTypeName(elementName)]), type);
    }
    
    [Fact]
    public void StringToType_Tuple_PredefinedTypes()
    {
        Assert.True(CSharpType.TryParse("(string, int)", out var type));
        Assert.Equal(new CSharpTypeTuple([new CSharpTupleElement(new CSharpTypeName("string")), new CSharpTupleElement(new CSharpTypeName("int"))]), type);
    }
    
    [Fact]
    public void StringToType_GenericList_Tuple_PredefinedTypes()
    {
        Assert.True(CSharpType.TryParse("List<(string, int)>", out var type));
        Assert.Equal(new CSharpTypeGeneric("List", [new CSharpTypeTuple([new CSharpTupleElement(new CSharpTypeName("string")), new CSharpTupleElement(new CSharpTypeName("int"))])]), type);
    }
    
    [Fact]
    public void StringToType_ComplexStructure()
    {
        Assert.True(CSharpType.TryParse("(decimal, Dictionary<string, (bool, List<DateTime>)>)", out var type));
        Assert.Equal(new CSharpTypeTuple([
                new CSharpTupleElement(new CSharpTypeName("decimal")),
                new CSharpTupleElement(new CSharpTypeGeneric("Dictionary", [
                    new CSharpTypeName("string"),
                    new CSharpTypeTuple([
                        new CSharpTupleElement(new CSharpTypeName("bool")),
                        new CSharpTupleElement(new CSharpTypeGeneric("List", [new CSharpTypeName("DateTime")]))
                    ])
                ]))
            ]), type);
    }

    // [Fact]
    // public async Task VariousReturnTypeMethods()
    // {
    //     var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
    //         .AddUsing("System")
    //         .AddClass("TestClass", @class =>
    //         {
    //             @class.AddMethod(new CSharpTypeVoid(), "MethodType_Void");
    //             @class.AddMethod(new CSharpTypeVoid().WrapInTask(), "MethodType_Task", method => method.Async());
    //             @class.AddMethod(new CSharpTypeName("string"), "MethodType_String", method => method
    //                 .AddStatement(@"return """";"));
    //             @class.AddMethod(new CSharpTypeName("string").WrapInList(), "MethodType_List_String", method => method
    //                 .AddStatement(@"return new List<string>();"));
    //             @class.AddMethod(new CSharpTypeName("string").WrapInList().WrapInTask(), "MethodType_Task_List_String", method => method
    //                 .Async()
    //                 .AddStatement(@"return new List<string>();"));
    //             @class.AddMethod(new CSharpTypeName("string").WrapInList().WrapInTask(), "MethodType_StrippedTask_List_String", method => method
    //                 .Sync()
    //                 .AddStatement(@"return new List<string>();"));
    //         })
    //         .CompleteBuild();
    //     await Verifier.Verify(fileBuilder.ToString());
    // }
}