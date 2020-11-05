using Intent.Modelers.Services.Api;
using Intent.Modules.AspNet.WebApi;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.Eventing;
using Intent.Templates;
using Intent.Metadata.Models;
using IApplication = Intent.Engine.IApplication;

namespace Intent.Modules.EntityFramework.Interop.WebApi.Decorators
{
    public class WebApiControllerDecorator : WebApiControllerDecoratorBase, IHasTemplateDependencies
    {
        private readonly IApplication _application;

        public WebApiControllerDecorator(IApplication application)
        {
            _application = application;
        }

        public const string Identifier = "Intent.EntityFramework.Interop.WebApi.ControllerDecorator";


        public override string DeclarePrivateVariables(ServiceModel service) => $@"
        private readonly {GetDbContextTemplate().ClassName} _dbContext;";

        public override string ConstructorParams(ServiceModel service) => $@"
            , {GetDbContextTemplate().ClassName} dbContext";

        public override string ConstructorInit(ServiceModel service) => @"
            _dbContext = dbContext;";

        public override string AfterCallToAppLayer(ServiceModel service, OperationModel operation)
        {
            if (operation.Stereotypes.Any(x => x.Name == "ReadOnly"))
            {
                return "";
            }
            return operation.IsAsync() 
                ? $@"
                    await _dbContext.SaveChangesAsync();"
                : $@"
                    _dbContext.SaveChanges();";
        }

        public override string OnDispose(ServiceModel service)
        {
            return @"
            _dbContext.Dispose();";
        }


        public override int Priority { get; set; } = -200;

        private IClassProvider GetDbContextTemplate()
        {
            return _application.FindTemplateInstance<IClassProvider>(TemplateDependency.OnTemplate(DbContextTemplate.Identifier));
        }

        public IEnumerable<ITemplateDependency> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependency.OnTemplate(DbContextTemplate.Identifier)
            };
        }
    }
}