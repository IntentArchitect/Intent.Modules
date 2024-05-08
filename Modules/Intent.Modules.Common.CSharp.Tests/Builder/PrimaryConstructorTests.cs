using System.Linq;
using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

[UsesVerify]
public class PrimaryConstructorTests
{
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
            .AddClass("Class", c =>
            {
                c.SetPrimaryConstructor();
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
        
        Assert.Empty(fileBuilder.Classes.First().Fields);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }

    [Fact]
    public async Task ClassWithSingleParamPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.SetPrimaryConstructor(ctor => ctor.AddClassParameter("string", "name"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
        
        Assert.Equal(1, fileBuilder.Classes.First().Fields.Count);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }
    
    [Fact]
    public async Task ClassWithFewParamsPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddClass("Class", c =>
            {
                c.SetPrimaryConstructor(ctor => ctor
                    .AddClassParameter("string", "name")
                    .AddClassParameter("string", "surname")
                    .AddClassParameter("string", "email")
                    .AddClassParameter("bool", "isActive"));
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
                c.SetPrimaryConstructor(ctor => ctor
                    .AddClassParameter("string", "name")
                    .AddClassParameter("string", "surname")
                    .AddClassParameter("string", "email")
                    .AddClassParameter("bool", "isActive")
                    .AddClassParameter("string", "mobileNumber")
                    .AddClassParameter("string", "homeNumber")
                    .AddClassParameter("string", "officeNumber")
                    .AddClassParameter("Gender", "gender", "Gender.Male"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
        
        Assert.Equal(8, fileBuilder.Classes.First().Fields.Count);
        Assert.Empty(fileBuilder.Classes.First().Properties);
    }
    
    // ======
    
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
            .AddRecord("Record", c =>
            {
                c.SetPrimaryConstructor();
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
        
        Assert.Empty(fileBuilder.Records.First().Properties);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }

    [Fact]
    public async Task RecordWithSingleParamPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record", c =>
            {
                c.SetPrimaryConstructor(ctor => ctor.AddRecordParameter("string", "Name"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
        
        Assert.Equal(1, fileBuilder.Records.First().Properties.Count);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }
    
    [Fact]
    public async Task RecordWithFewParamsPrimaryConstructor()
    {
        var fileBuilder = new CSharpFile("Namespace", "File")
            .AddRecord("Record", c =>
            {
                c.SetPrimaryConstructor(ctor => ctor
                    .AddRecordParameter("string", "Name")
                    .AddRecordParameter("string", "Surname")
                    .AddRecordParameter("string", "Email")
                    .AddRecordParameter("bool", "IsActive"));
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
                c.SetPrimaryConstructor(ctor => ctor
                    .AddRecordParameter("string", "Name")
                    .AddRecordParameter("string", "Surname")
                    .AddRecordParameter("string", "Email")
                    .AddRecordParameter("bool", "IsActive")
                    .AddRecordParameter("string", "MobileNumber")
                    .AddRecordParameter("string", "HomeNumber")
                    .AddRecordParameter("string", "OfficeNumber")
                    .AddRecordParameter("Gender", "Gender", "Gender.Male"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
        
        Assert.Equal(8, fileBuilder.Records.First().Properties.Count);
        Assert.Empty(fileBuilder.Records.First().Fields);
    }
}