using System.Collections.Generic;
using Intent.Metadata.Models;
using Intent.Modelers.Services.Api;
using Intent.Templates;

namespace Intent.Modules.AspNetCore.WebApi.Templates.Controller
{
    public abstract class WebApiControllerDecoratorBase : ITemplateDecorator
    {
        public virtual IEnumerable<string> DeclareUsings(IServiceModel service) => new List<string>();

        public virtual string DeclarePrivateVariables(IServiceModel service) => string.Empty;

        public virtual string ConstructorParams(IServiceModel service) => string.Empty;

        public virtual string ConstructorInit(IServiceModel service) => string.Empty;

        public virtual IEnumerable<string> PayloadPropertyDecorators(IOperationParameter parameter) => new string[] {};

        public virtual string BeginOperation(IServiceModel service, IOperation operation) => string.Empty;

        public virtual string BeforeTransaction(IServiceModel service, IOperation operation) => string.Empty;

        public virtual string BeforeCallToAppLayer(IServiceModel service, IOperation operation) => string.Empty;

        public virtual string AfterCallToAppLayer(IServiceModel service, IOperation operation) => string.Empty;

        public virtual string AfterTransaction(IServiceModel service, IOperation operation) => string.Empty;

        public virtual string OnExceptionCaught(IServiceModel service, IOperation operation) => string.Empty;

        public virtual bool HandlesCaughtException() => false;

        public virtual string OnDispose(IServiceModel service) => string.Empty;

        public virtual string ClassMethods(IServiceModel service) => string.Empty;

        public virtual int Priority { get; set; } = 0;
    }
}