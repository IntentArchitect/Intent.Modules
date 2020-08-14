using Intent.Templates;
using System;
using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Modules.Common;

namespace Intent.Modules.Application.ServiceImplementations.Templates.ServiceImplementation
{
    public abstract class ServiceImplementationDecoratorBase : ITemplateDecorator
    {
        public virtual IEnumerable<string> GetUsings(ServiceModel service)
        {
            return new List<string>();
        }

        public virtual IEnumerable<ConstructorParameter> GetConstructorDependencies(ServiceModel service)
        {
            return new List<ConstructorParameter>();
        }

        public virtual string GetDecoratedImplementation(ServiceModel service, OperationModel operationModel)
        {
            return string.Empty;
        }

        public int Priority { get; } = 0;
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
