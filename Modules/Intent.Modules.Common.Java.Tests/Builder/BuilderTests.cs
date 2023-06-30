using Intent.Modules.Common.Java.Builder;

namespace Intent.Modules.Common.Java.Tests.Builder;

using Xunit;
using VerifyXunit;

[UsesVerify]
public class BuilderTests
{
    [Fact]
    public async Task BuildRestServiceImplementationTest()
    {
        var fileBuilder = new JavaFile("com.spring_petclinic.spring_petclinic_rest.application.services.impl", "")
            .AddImport("lombok.AllArgsConstructor")
            .AddImport("java.util.List")
            .AddClass("OwnerRestServiceImpl", c =>
            {
                c.AddAnnotation("Service")
                    .AddAnnotation("AllArgsConstructor")
                    .AddAnnotation("IntentMerge");
                c.ImplementsInterface("OwnerRestService");
                c.AddField("OwnerRepository", "ownerRepository")
                    .AddField("ModelMapper", "mapper");
                c.AddMethod("List<OwnerDTO>", "getOwner", method => method
                    .AddAnnotation("Override")
                    .AddAnnotation("Transactional", ann => ann.AddArgument("readOnly = true"))
                    .AddAnnotation("IntentIgnoreBody")
                    .AddStatement("var owners = ownerRepository.findAll();")
                    .AddStatement("return OwnerDTO.mapFromOwners(owners, mapper);"));
                c.AddMethod("void", "addOwner", method => method
                    .AddParameter("OwnerCreateDTO", "dto")
                    .AddAnnotation("Override")
                    .AddAnnotation("Transactional", ann => ann.AddArgument("readOnly = false"))
                    .AddAnnotation("IntentIgnoreBody")
                    .AddStatement("var owner = new Owner();")
                    .AddStatement("owner.setFirstName(dto.getFirstName());")
                    .AddStatement("owner.setLastName(dto.getLastName());")
                    .AddStatement("owner.setAddress(dto.getAddress());")
                    .AddStatement("owner.setCity(dto.getCity());")
                    .AddStatement("owner.setTelephone(dto.getTelephone());")
                    .AddStatement("ownerRepository.save(owner);"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task BuildModifiers()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddImport("java.util.List")
            .AddClass("TestClass", c =>
            {
                c.Final();
                c.AddMethod("void", "main", method => method
                    .Static()
                    .AddParameter("String", "args[]"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task StatementBlocks()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddClass("TestClass", c =>
            {
                c.Final();
                c.AddMethod("void", "TestStatements", method => method
                    .Static()
                    .AddParameter("String", "testParam")
                    .AddStatement(new JavaStatementBlock()
                        .AddStatement("// Simple block"))
                    .AddStatement(new JavaStatementBlock(@"if (testParam == """")")
                        .AddStatement("throw new IllegalArgumentException();")));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task JavadocComments()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddClass("TestClass", c =>
            {
                c.AddMethod("Image", "getImage", method =>
                {
                    method.WithComments(@"
Returns an Image object that can then be painted on the screen. 
The url argument must specify an absolute <a href=""#{@link}"">{@link URL}</a>. The name
argument is a specifier that is relative to the url argument. 
<p>
This method always returns immediately, whether or not the 
image exists. When this applet attempts to draw the image on
the screen, the data will be loaded. The graphics primitives 
that draw the image will incrementally paint on the screen. 

@param  url  an absolute URL giving the base location of the image
@param  name the location of the image, relative to the url argument
@return      the image at the specified URL
@see         Image");
                    method.AddParameter("URL", "url");
                    method.AddParameter("String", "name");
                    method.AddStatement(new JavaStatementBlock("try")
                            .AddStatement("return getImage(new URL(url, name));"))
                        .AddStatement(new JavaStatementBlock("catch (MalformedURLException e)")
                            .AddStatement("return null;"));
                });
                c.AddMethod("void", "Basic", method =>
                {
                    method.WithComments(@"/**
* This is a test
*/")
                        .CommentWithoutDelimiters();
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task CodeBlocks()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddClass("TestClass", c =>
            {
                c.AddCodeBlock($@"// This is a free flow code block
public void TestMethod()
{{
}}");
            })
            .AddInterface("TestInterface", i =>
            {
                i.AddCodeBlock($@"// This is a free flow code block
void TestMethod();");
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task Finals()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddClass("TestClass", c =>
            {
                c.Final();
                c.AddField("String", "someName", param => param.Final());
                c.AddField("String", "someOtherName", param => param.PrivateFinal());
                c.AddMethod("void", "testMethod", method =>
                {
                    method.Final();
                    method.AddParameter("String", "message", param => param.Final());
                });
                c.AddMethod("void", "testMethodTwo", method =>
                {
                    method.Final();
                    method.AddParameter("String", "message",
                        param => param.Final().AddAnnotation("Value", a => a.AddArgument(@"""value""")));
                });
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task Generics()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddInterface("TestInterface", i =>
            {
                i.AddGenericParameter("S", out var s);
                i.AddGenericParameter("T", out var t);
                i.AddMethod("void", "isMethodWithType", method => method
                    .AddGenericParameter("U", out var u)
                    .AddGenericParameter("V", out var v)
                    .AddParameter(s, "paramS")
                    .AddParameter(t, "paramT")
                    .AddParameter(u, "paramU")
                    .AddParameter(v, "paramV"));
            })
            .AddClass("TestClass", c =>
            {
                c.AddGenericParameter("S", out var s);
                c.AddGenericParameter("T", out var t);
                c.WithBaseType($"TestInterface<{s}, {t}>");
                c.AddMethod("void", "isMethodWithType", method => method
                    .AddGenericParameter("U", out var u)
                    .AddGenericParameter("V", out var v)
                    .AddParameter(s, "paramS")
                    .AddParameter(t, "paramT")
                    .AddParameter(u, "paramU")
                    .AddParameter(v, "paramV"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task Fields()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddClass("TestClass", c =>
            {
                c.AddField("String", "normalField");
                c.AddField("String", "privateFinalField", f => f.PrivateFinal());
                c.AddField("String", "publicField", f => f.Public());
                c.AddField("String", "publicFinalField", f => f.PublicFinal());
                c.AddField("String", "fieldWithValue", f => f.WithDefaultValue(@"""test value"""));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task InterfaceMethods()
    {
        var fileBuilder = new JavaFile("com.test", "")
            .AddInterface("TestInterface", i =>
            {
                i.AddMethod("void", "normalMethod",
                    method => method.AddParameter("String", "value").AddAnnotation("@TestAnnotation"));
                i.AddMethod("String", "methodWithBody",
                    method => method.AddStatement(@"return """";").AddAnnotation("@TestAnnotation"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}