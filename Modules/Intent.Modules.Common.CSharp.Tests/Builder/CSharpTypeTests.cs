using Intent.Modules.Common.CSharp.Builder;
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