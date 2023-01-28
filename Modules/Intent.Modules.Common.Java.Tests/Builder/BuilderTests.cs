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
                c.AddField("OwnerRepository", "ownerRepository", field => field.Private())
                    .AddField("ModelMapper", "mapper", field => field.Private());
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
}