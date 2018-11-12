using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Service;
using Intent.Modules.Application.Contracts;
using Intent.Modules.AspNetCore.WebApi.Templates.Controller;
using Intent.Modules.EntityFrameworkCore.Templates.DbContext;
using Intent.SoftwareFactory.Engine;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.EntityFrameworkCore.Interop.WebApi.Decorators
{
    public class WebApiControllerDecorator : WebApiControllerDecoratorBase, IHasTemplateDependencies
    {
        private readonly IApplication _application;

        public WebApiControllerDecorator(IApplication application)
        {
            _application = application;
        }

        public const string Identifier = "Intent.EntityFrameworkCore.Interop.WebApi.ControllerDecorator";

        public override string DeclarePrivateVariables(IServiceModel service) => $@"
        private readonly {GetDbContextTemplate().ClassName} _dbContext;";

        public override string ConstructorParams(IServiceModel service) => $@"
            , {GetDbContextTemplate().ClassName} dbContext";

        public override string ConstructorInit(IServiceModel service) => @"
            _dbContext = dbContext;";

        public override string AfterCallToAppLayer(IServiceModel service, IOperationModel operation)
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

        public override string OnDispose(IServiceModel service)
        {
            return @"
            _dbContext.Dispose();";
        }


        public override int Priority { get; set; } = -200;

        private IHasClassDetails GetDbContextTemplate()
        {
            return _application.FindTemplateInstance<IHasClassDetails>(TemplateDependancy.OnTemplate(DbContextTemplate.Identifier));
        }

        public IEnumerable<ITemplateDependancy> GetTemplateDependencies()
        {
            return new[]
            {
                TemplateDependancy.OnTemplate(DbContextTemplate.Identifier)
            };
        }
    }
}