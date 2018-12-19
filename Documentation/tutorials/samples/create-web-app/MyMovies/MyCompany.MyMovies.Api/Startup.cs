using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyCompany.MyMovies.Infrastructure.Data;
using Swashbuckle.AspNetCore.Swagger;
using MyCompany.MyMovies.Domain;
using MyCompany.MyMovies.Application;
using MyCompany.MyMovies.Application.ServiceImplementation;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AspNetCore.Startup", Version = "1.0")]

namespace MyCompany.MyMovies.Api
{
    [IntentManaged(Mode.Merge)]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // [IntentManaged(Mode.Ignore)] // Uncomment this line to take over management of configuring services
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            IntentConfiguredServices(services);

            ConfigureSwagger(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            InitializeSwagger(app);
        }

        public void IntentConfiguredServices(IServiceCollection services)
        {

            ConfigureDbContext(services);
            services.AddTransient<IMovieManager, MovieManager>();
            services.AddTransient<IMovieRepository, MovieRepository>();
        }



        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void InitializeSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyMovies API V1");
            });
        }

        //[IntentManaged(Mode.Ignore)] // Uncomment to take control of this method.
        private void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MyMovies API", Version = "v1" });
            });
        }

        [IntentManaged(Mode.Ignore)] // We're taking this method over
        private void ConfigureDbContext(IServiceCollection services)
        {
            services.AddDbContext<MyMoviesDbContext>(x =>
                x.UseInMemoryDatabase("MyCompany.MyMovies"));
        }
    }
}