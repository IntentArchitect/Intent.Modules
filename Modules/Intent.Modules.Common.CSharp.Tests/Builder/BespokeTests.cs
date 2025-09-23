using System.Threading.Tasks;
using Intent.Modules.Common.CSharp.Builder;
using VerifyXunit;
using Xunit;

namespace Intent.Modules.Common.CSharp.Tests.Builder;

public class BespokeTests
{
    [Fact]
    public async Task StaticConfigureStyleFileBuilderTest()
    {
        var fileBuilder = new CSharpFile("Testing.Namespace", "RelativeLocation")
            .AddUsing("System")
            .AddUsing("Azure")
            .AddUsing("Azure.Messaging.EventGrid")
            .AddUsing("Azure.Messaging.ServiceBus")
            .AddUsing("Microsoft.EntityFrameworkCore")
            .AddUsing("Microsoft.Extensions.Configuration")
            .AddUsing("Microsoft.Extensions.DependencyInjection")
            .AddClass("DependencyInjection", @class =>
            {
                @class.Static()
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
                                .WithExpressionBody(@"provider.GetService<ApplicationDbContext>()"))));
            })
            .CompleteBuild();
        await Verifier.Verify(fileBuilder.ToString());
    }
}