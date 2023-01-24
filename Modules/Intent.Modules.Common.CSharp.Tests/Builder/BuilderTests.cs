using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using Xunit;
using VerifyXunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

[UsesVerify]
public class BuilderTests
{
    [Fact]
    public async Task BasicClassBuilderTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddUsing("System")
            .AddUsing("System.Collections.Generic")
            .AddClass("Vehicle", c =>
            {
                c.AddConstructor(ctor => ctor
                        .AddParameter("string", "make")
                        .AddParameter("string", "model")
                        .AddParameter("int", "year")
                        .AddStatement("Make = make;")
                        .AddStatement("Model = model;")
                        .AddStatement("Year = year;"))
                    .AddProperty("string", "Make")
                    .AddProperty("string", "Model")
                    .AddProperty("int", "Year")
                    .AddMethod("void", "StartEngine", method => method
                        .AddStatement(@"Console.WriteLine(""The engine is running."");"))
                    .AddMethod("void", "Drive", method => method
                        .WithComments("// Method with parameters")
                        .AddStatement(@"Console.WriteLine($""The {Make} {Model} drove {distance} miles at {speed} mph."");")
                        .AddParameter("int", "distance")
                        .AddParameter("int", "speed"))
                    .AddMethod("int", "CalculateAge", method => method
                        .WithComments("// Method with return value")
                        .AddStatement("int currentYear = DateTime.Now.Year;")
                        .AddStatement("return currentYear - Year;"))
                    .AddMethod("string", "ToString", method => method
                        .WithComments("// Method that overrides a virtual method")
                        .Override()
                        .AddStatement(@"return $""{Make} {Model} ({Year})"";"));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }

    [Fact]
    public async Task StaticConfigureStyleFileBuilderTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "Classes")
            .AddUsing("System")
            .AddUsing("Azure")
            .AddUsing("Azure.Messaging.EventGrid")
            .AddUsing("Azure.Messaging.ServiceBus")
            .AddUsing("Microsoft.EntityFrameworkCore")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass("DependencyInjection", c =>
            {
                c.Static()
                    .AddMethod("IServiceCollection", "AddInfrastructure", method => method
                        .Static()
                        .AddParameter("IServiceCollection", "services", param => param.WithThisModifier())
                        .AddParameter("IConfiguration", "configuration")
                        .AddStatement(new CSharpInvocationStatement("services.AddDbContext<ApplicationDbContext>")
                            .AddArgument(new CSharpLambdaBlock("(sp, options)")
                                .AddStatement(@"options.UseInMemoryDatabase(""DefaultConnection"");")
                                .AddStatement(@"options.UseLazyLoadingProxies();"))
                            .WithArgumentsOnNewLines())
                        .AddStatement(new CSharpInvocationStatement(@"services.AddScoped<IUnitOfWork>")
                            .AddArgument(new CSharpLambdaBlock("provider")
                                .AddStatement(@"provider.GetService<ApplicationDbContext>());"))
                            .WithArgumentsOnNewLines()));

            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}