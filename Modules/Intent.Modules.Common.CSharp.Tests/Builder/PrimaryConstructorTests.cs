using System.Linq;
using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class PrimaryConstructorTests
{
    // ====== Classes ======
    
    [Fact]
    public async Task ClassWithoutAnySignatureOrMember()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class")
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ClassWithEmptyPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c => { c.AddPrimaryConstructor(); })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ClassWithSingleParamPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c => { c.AddPrimaryConstructor(ctor => ctor.AddParameter("string", "name")); })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Single(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ClassWithFewParamsPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "surname")
                    .AddParameter("string", "email")
                    .AddParameter("bool", "isActive")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Equal(4, fileBuilder.Classes.First().Fields.Count);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ClassWithLotsParamsPrimaryConstructor_SpansLines()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "surname")
                    .AddParameter("string", "email")
                    .AddParameter("bool", "isActive")
                    .AddParameter("string", "mobileNumber")
                    .AddParameter("string", "homeNumber")
                    .AddParameter("string", "officeNumber")
                    .AddParameter("Gender", "gender", param => param.WithDefaultValue("Gender.Male"))
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Equal(8, fileBuilder.Classes.First().Fields.Count);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ClassWithPrimaryConstructorAndMembers()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "surname")
                    .AddParameter("string", "email")
                    .AddParameter("bool", "isActive")
                );
                c.AddConstructor(ctor => ctor.AddParameter("string", "anotherName", param => param.IntroduceProperty()).CallsThis());
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Equal(4, fileBuilder.Classes.First().Fields.Count);
        Assert.Single(fileBuilder.Classes.First().Properties);
    }
    
    [Fact]
    public async Task ClassWithPrimaryConstructorAndInheritedBaseCall()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("SomeBaseClass", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor.AddParameter("string", "someValue"));
            })
            .AddClass("Class", c =>
            {
                c.WithBaseType("SomeBaseClass");
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "name")
                    .AddParameter("string", "surname")
                    .AddParameter("string", "email")
                    .AddParameter("bool", "isActive")
                    .CallsBase(b => b.AddArgument(@"""some value here"""))
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Equal(4, fileBuilder.Classes.Last().Fields.Count);
        Assert.Empty(fileBuilder.Classes.Last().Properties);
    }
    
    // ====== Records ======

    [Fact]
    public async Task RecordWithoutAnySignatureOrMember()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record")
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Records.First().Properties);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }

    [Fact]
    public async Task RecordWithEmptyPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record", c => { c.AddPrimaryConstructor(); })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Records.First().Properties);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }

    [Fact]
    public async Task RecordWithSingleParamPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record", c => { c.AddPrimaryConstructor(ctor => ctor.AddParameter("string", "Name")); })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Single(fileBuilder.Records.First().Properties);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }

    [Fact]
    public async Task RecordWithFewParamsPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "Name")
                    .AddParameter("string", "Surname")
                    .AddParameter("string", "Email")
                    .AddParameter("bool", "IsActive")
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Equal(4, fileBuilder.Records.First().Properties.Count);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }

    [Fact]
    public async Task RecordWithLotsParamsPrimaryConstructor_SpansLines()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "Name")
                    .AddParameter("string", "Surname")
                    .AddParameter("string", "Email")
                    .AddParameter("bool", "IsActive")
                    .AddParameter("string", "MobileNumber")
                    .AddParameter("string", "HomeNumber")
                    .AddParameter("string", "OfficeNumber")
                    .AddParameter("Gender", "Gender", param => param.WithDefaultValue("Gender.Male"))
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Equal(8, fileBuilder.Records.First().Properties.Count);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }
    
    [Fact]
    public async Task RecordWithPrimaryConstructorAndMembers()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "Name")
                    .AddParameter("string", "Surname")
                    .AddParameter("string", "Email")
                    .AddParameter("bool", "IsActive")
                );
                c.AddConstructor(ctor => ctor.AddParameter("string", "anotherName", param => param.IntroduceProperty()).CallsThis());
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Records.First().Fields);
        Assert.Equal(5, fileBuilder.Records.First().Properties.Count);
    }
    
    [Fact]
    public async Task RecordWithPrimaryConstructorAndInheritedBaseCall()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("SomeBaseRecord", c =>
            {
                c.AddPrimaryConstructor(ctor => ctor.AddParameter("string", "SomeValue"));
            })
            .AddRecord("Record", c =>
            {
                c.WithBaseType("SomeBaseRecord");
                c.AddPrimaryConstructor(ctor => ctor
                    .AddParameter("string", "Name")
                    .AddParameter("string", "Surname")
                    .AddParameter("string", "Email")
                    .AddParameter("bool", "IsActive")
                    .CallsBase(b => b.AddArgument(@"""some value here"""))
                );
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());

        Assert.Empty(fileBuilder.Records.Last().Fields);
        Assert.Equal(4, fileBuilder.Records.Last().Properties.Count);
    }
}