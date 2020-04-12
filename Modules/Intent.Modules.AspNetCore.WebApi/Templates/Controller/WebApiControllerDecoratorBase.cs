using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.WebApi.Templates.Controller
{
    public abstract class WebApiControllerDecoratorBase : ITemplateDecorator
    {
        public virtual IEnumerable<string> DeclareUsings(ServiceModel service) => new List<string>();

        public virtual string DeclarePrivateVariables(ServiceModel service) => string.Empty;

        public virtual string ConstructorParams(ServiceModel service) => string.Empty;

        public virtual string ConstructorInit(ServiceModel service) => string.Empty;

        public virtual IEnumerable<string> PayloadPropertyDecorators(ParameterModel parameter) => new string[] {};

        public virtual string BeginOperation(ServiceModel service, OperationModel operation) => string.Empty;

        public virtual string BeforeTransaction(ServiceModel service, OperationModel operation) => string.Empty;

        public virtual string BeforeCallToAppLayer(ServiceModel service, OperationModel operation) => string.Empty;

        public virtual string AfterCallToAppLayer(ServiceModel service, OperationModel operation) => string.Empty;

        public virtual string AfterTransaction(ServiceModel service, OperationModel operation) => string.Empty;

        public virtual string OnExceptionCaught(ServiceModel service, OperationModel operation) => string.Empty;

        public virtual bool HandlesCaughtException() => false;

        public virtual string OnDispose(ServiceModel service) => string.Empty;

        public virtual string ClassMethods(ServiceModel service) => string.Empty;

        public virtual int Priority { get; set; } = 0;
    }
}