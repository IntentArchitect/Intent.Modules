using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public abstract class ServiceImplementationDecoratorBase : ITemplateDecorator
    {
        public virtual string GetUsings()
        {
            return string.Empty;
        }

        public virtual IEnumerable<ConstructorParameter> GetConstructorDependencies(IServiceModel serviceModel)
        {
            return new List<ConstructorParameter>();
        }

        public virtual string GetDecoratedImplementation(IServiceModel serviceModel, IOperationModel operationModel)
        {
            return string.Empty;
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
