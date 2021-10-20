using System.Collections.Generic;
using Intent.Engine;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    internal interface IFastLookupTemplateDependency : ITemplateDependency
    {
        ITemplate LookupTemplateInstance(ISoftwareFactoryExecutionContext context);
        IEnumerable<ITemplate> LookupTemplateInstances(ISoftwareFactoryExecutionContext context);
        IOutputTarget LookupOutputTarget(ISoftwareFactoryExecutionContext context);
    }
}