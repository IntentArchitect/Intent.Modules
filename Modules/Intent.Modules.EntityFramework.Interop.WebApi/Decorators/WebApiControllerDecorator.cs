using Intent.MetaModel.Service;
using Intent.Modules.AspNet.WebApi;
using Intent.Modules.AspNet.WebApi.Templates.Controller;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Application.Contracts;
using Intent.Modules.Common;
using Intent.Modules.Common.Templates;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.SoftwareFactory.Eventing;
using Intent.SoftwareFactory.Templates;
using Intent.SoftwareFactory.Engine;

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


        public override string DeclarePrivateVariables(IServiceModel service) => HasExplicitTransactionScope(service) ? $@"
        private readonly {GetDbContextTemplate().ClassName} _dbContext;" : string.Empty;

        public override string ConstructorParams(IServiceModel service) => HasExplicitTransactionScope(service) ? $@"
            , {GetDbContextTemplate().ClassName} dbContext" : string.Empty;

        public override string ConstructorInit(IServiceModel service) => HasExplicitTransactionScope(service) ? @"
            _dbContext = dbContext;" : string.Empty;

        public override string AfterCallToAppLayer(IServiceModel service, IOperationModel operation)
        {
            if (!ShouldHaveTransactionScope(operation))
            {
                return string.Empty;
            }
            if (operation.Stereotypes.Any(x => x.Name == "ReadOnly"))
            {
                return string.Empty;
            }
            return operation.IsAsync() 
                ? $@"
                    await _dbContext.SaveChangesAsync();"
                : $@"
                    _dbContext.SaveChanges();";
        }

        public override string OnDispose(IServiceModel service)
        {
            if (!HasExplicitTransactionScope(service))
            {
                return string.Empty;
            }
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

        private bool HasExplicitTransactionScope(IServiceModel service)
        {
            return service.Operations.Any(x => x.GetStereotypeProperty<bool>("Transactional Settings", "Explicit Scope", true));
        }

        private bool ShouldHaveTransactionScope(IOperationModel operation)
        {
            return operation.GetStereotypeProperty<bool>("Transactional Settings", "Explicit Scope", true);
        }
    }
}