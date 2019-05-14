using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Templates;
using System;
using System.Collections.Generic;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public abstract class ServiceImplementationDecoratorBase : ITemplateDecorator
    {
        public virtual IEnumerable<string> GetUsings(IServiceModel serviceModel)
        {
            return new List<string>();
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
        public ConstructorParameter(string type, string name, ITemplateDependancy templateDependency)
        {
            ParameterType = type;
            ParameterName = name;
            TemplateDependency = templateDependency;
        }

        public string ParameterType { get; }
        public string ParameterName { get; }
        public ITemplateDependancy TemplateDependency { get; }
    }
}
