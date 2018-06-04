using System.ComponentModel;
using Intent.Modules.EntityFramework.Templates.DbContext;
using Intent.SoftwareFactory.Registrations;

namespace Intent.Modules.EntityFramework.Repositories.Decorators
{
    [Description(DbContextDecorator.Identifier)]
    public class DbContextDecoratorRegistration : DecoratorRegistration<DbContextDecoratorBase>
    {

        public override string DecoratorId => DbContextDecorator.Identifier;

        public override object CreateDecoratorInstance()
        {
            return new DbContextDecorator();
        }
    }
}
