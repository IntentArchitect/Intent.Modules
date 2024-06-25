using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class ReturnTypeTests
{
    [Fact]
    public void ReturnType_Void()
    {
        Assert.Equal("void", new CSharpReturnTypeVoid().GetText(""));
    }

    [Fact]
    public void ReturnType_String()
    {
        Assert.Equal("string", new CSharpReturnTypeName("string").GetText(""));
    }

    [Fact]
    public void ReturnType_Task()
    {
        Assert.Equal("Task", new CSharpReturnTypeName("Task").GetText(""));
    }

    [Fact]
    public void ReturnType_Tuple_WithNames()
    {
        Assert.Equal("(string Name, string Surname)", new CSharpReturnTypeTuple([
            new CSharpTupleElement(new CSharpReturnTypeName("string"), "Name"),
            new CSharpTupleElement(new CSharpReturnTypeName("string"), "Surname")
        ]).GetText(""));
    }

    [Fact]
    public void ReturnType_Tuple_WithoutNames()
    {
        Assert.Equal("(string, string)", new CSharpReturnTypeTuple([
            new CSharpTupleElement(new CSharpReturnTypeName("string")),
            new CSharpTupleElement(new CSharpReturnTypeName("string"))
        ]).GetText(""));
    }

    [Fact]
    public void ReturnType_List_String()
    {
        Assert.Equal("List<string>", new CSharpReturnTypeGeneric("List", [new CSharpReturnTypeName("string")]).GetText(""));
    }

    [Fact]
    public void ReturnType_Task_Bool()
    {
        Assert.Equal("Task<bool>", new CSharpReturnTypeGeneric("Task", [new CSharpReturnTypeName("bool")]).GetText(""));
    }

    [Fact]
    public void ReturnType_Task_Tuple()
    {
        Assert.Equal("Task<(bool, string)>", new CSharpReturnTypeGeneric("Task", [
            new CSharpReturnTypeTuple([
                new CSharpTupleElement(new CSharpReturnTypeName("bool")),
                new CSharpTupleElement(new CSharpReturnTypeName("string"))
            ])
        ]).GetText(""));
    }

    [Fact]
    public void ReturnType_Task_List_Int()
    {
        Assert.Equal("Task<List<int>>", new CSharpReturnTypeGeneric("Task", [
            new CSharpReturnTypeGeneric("List", [new CSharpReturnTypeName("int")])
        ]).GetText(""));
    }

    [Fact]
    public void ReturnType_Task_List_Tuple()
    {
        Assert.Equal("Task<List<(bool, string)>>", new CSharpReturnTypeGeneric("Task", [
            new CSharpReturnTypeGeneric("List", [
                new CSharpReturnTypeTuple([
                    new CSharpTupleElement(new CSharpReturnTypeName("bool")),
                    new CSharpTupleElement(new CSharpReturnTypeName("string"))
                ])
            ])
        ]).GetText(""));
    }

    [Fact]
    public async Task VariousReturnTypeMethods()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddClass("TestClass", @class =>
            {
                @class.AddMethod(new CSharpReturnTypeVoid(), "MethodReturnType_Void");
                @class.AddMethod(new CSharpReturnTypeVoid().WrapInTask(), "MethodReturnType_Task", method => method.Async());
                @class.AddMethod(new CSharpReturnTypeName("string"), "MethodReturnType_String", method => method
                    .AddStatement(@"return """";"));
                @class.AddMethod(new CSharpReturnTypeName("string").WrapInList(), "MethodReturnType_List_String", method => method
                    .AddStatement(@"return new List<string>();"));
                @class.AddMethod(new CSharpReturnTypeName("string").WrapInList().WrapInTask(), "MethodReturnType_Task_List_String", method => method
                    .Async()
                    .AddStatement(@"return new List<string>();"));
                @class.AddMethod(new CSharpReturnTypeName("string").WrapInList().WrapInTask(), "MethodReturnType_StrippedTask_List_String", method => method
                    .Sync()
                    .AddStatement(@"return new List<string>();"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}