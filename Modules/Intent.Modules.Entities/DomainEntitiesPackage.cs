using System.Collections.Generic;
using Inoxico.Automation.Plugins.Packages.Domain.Decorators;
using Inoxico.Automation.Plugins.Packages.Domain.Templates.DomainEntity;
using Inoxico.Automation.Plugins.Packages.Domain.Templates.DomainEntityBehaviour;
using Inoxico.Automation.Plugins.Packages.Domain.Templates.DomainEntityInterface;
using Intent.SoftwareFactory.Configuration;
using Intent.SoftwareFactory.Packages;

namespace Inoxico.Automation.Plugins.Packages.Domain
{
    public class DomainEntitiesPackage : ApplicationPackage
    {
        public DomainEntitiesPackage()
        {
            PackageTemplates = new List<PackageTemplate>()
            {
                new PackageTemplate(TemplateOutputConfig.Create(nameof(DomainEntityTemplate)), ProjectRole.Domain),
                new PackageTemplate(TemplateOutputConfig.Create(nameof(DomainEntityInterfaceTemplate)), ProjectRole.Domain),
                new PackageTemplate(TemplateOutputConfig.Create(nameof(DomainEntityBehaviourTemplate)), ProjectRole.Domain)
            };
            PackageDecorators = new List<PackageDecorator>()
            {
                new PackageDecorator(nameof(DDDEntityDecorator)),
                new PackageDecorator(nameof(DDDEntityInterfaceDecorator)),
                new PackageDecorator(nameof(SerializableEntityDecorator)),
                new PackageDecorator(nameof(SerializableEntityInterfaceDecorator))
            };
        }
    }
}