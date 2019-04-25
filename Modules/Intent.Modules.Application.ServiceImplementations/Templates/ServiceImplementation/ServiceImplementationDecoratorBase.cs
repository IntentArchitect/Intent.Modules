using Intent.Templates;
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;

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

        public virtual string GetDecoratedImplementation(IServiceModel serviceModel, IOperation operationModel)
        {
            return string.Empty;
        }
    }

    public class ConstructorParameter
    {
        public ConstructorParameter(string type, string name, ITemplateDependency templateDependency)
        {
            ParameterType = type;
            ParameterName = name;
            TemplateDependency = templateDependency;
        }

        public string ParameterType { get; }
        public string ParameterName { get; }
        public ITemplateDependency TemplateDependency { get; }
    }
}
