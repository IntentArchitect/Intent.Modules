using Accelerators.Domain.Common.Interfaces;
using Accelerators.Domain.Repositories.DTOFieldSync;
using Accelerators.Infrastructure.Persistence;
using Accelerators.Infrastructure.Repositories.DTOFieldSync;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Infrastructure.DependencyInjection.DependencyInjection", Version = "1.0")]

namespace Accelerators.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseInMemoryDatabase("DefaultConnection");
                options.UseLazyLoadingProxies();
            });
            services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
            services.AddTransient<IBlock1Level1Repository, Block1Level1Repository>();
            services.AddTransient<IBlock2Level1Repository, Block2Level1Repository>();
            services.AddTransient<IShouldNotSeeRepository, ShouldNotSeeRepository>();
            return services;
        }
    }
}