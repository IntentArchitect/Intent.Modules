using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class AsyncMethods
{
    [Fact]
    public async Task ClassMethodAsync_Task()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async();
                    m.AddNotImplementedException();
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task InterfaceMethodAsync_Task()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddInterface("Interface", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async();
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task ClassMethodAsync_ValueTask()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async(true);
                    m.AddNotImplementedException();
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
    
    [Fact]
    public async Task InterfaceMethodAsync_ValueTask()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddInterface("Interface", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async(true);
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task ClassMethodAsync_TaskToValueTask()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async(); // Keep this for testing later conversion.
                    m.Async(true);
                    m.AddNotImplementedException();
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task ClassMethodAsync_ValueTaskToTask()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async(true); // Keep this for testing later conversion.
                    m.Async();
                    m.AddNotImplementedException();
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task InterfaceMethodAsync_TaskToValueTask()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddInterface("Interface", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async(); // Keep this for testing later conversion.
                    m.Async(true);
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
    
    [Fact]
    public async Task InterfaceMethodAsync_ValueTaskToTask()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddInterface("Interface", c =>
            {
                c.AddMethod("string", "GetNameAsync", m =>
                {
                    m.Async(true); // Keep this for testing later conversion.
                    m.Async();
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    } 
}