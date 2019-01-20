using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public abstract class ServiceImplementationDecoratorBase : ITemplateDecorator
    {
        // DJVV - TODO:
        // Convention module:
        // 1. Scan MetadataManger in the same way the Repo Interface register does it
        // 2. This will mean that you will need the module settings like the Repo Interface module (filtering and TemplateID)
        // 3. Then you can return those interface FQDNs with parameter names for the Constructor dependencies
        // 4. Determining usings are dependent on the current service implementation
        // 5. Now you can match up domain classes with services and populate the relevant implementations

        public virtual string GetUsings()
        {
            return string.Empty;
        }

        public virtual string GetDecoratedImplementation(IOperationModel operationModel)
        {
            return string.Empty;
        }

        public virtual IEnumerable<ConstructorParameter> GetConstructorDependencies()
        {
            return new List<ConstructorParameter>();
        }
    }

    public class ConstructorParameter
    {
        public ConstructorParameter(string type, string name)
        {
            ParameterType = type;
            ParameterName = name;
        }

        public string ParameterType { get; }
        public string ParameterName { get; }
    }
}
