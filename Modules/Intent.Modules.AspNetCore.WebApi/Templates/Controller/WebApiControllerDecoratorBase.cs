using System.Collections.Generic;
using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.AspNetCore.WebApi.Templates.Controller
{
    public abstract class WebApiControllerDecoratorBase : ITemplateDecorator, IPriorityDecorator
    {
        public virtual IEnumerable<string> DeclareUsings(IServiceModel service) => new List<string>();

        public virtual string DeclarePrivateVariables(IServiceModel service) => string.Empty;

        public virtual string ConstructorParams(IServiceModel service) => string.Empty;

        public virtual string ConstructorInit(IServiceModel service) => string.Empty;

        public virtual IEnumerable<string> PayloadPropertyDecorators(IOperationParameterModel parameter) => new string[] {};

        public virtual string BeginOperation(IServiceModel service, IOperationModel operation) => string.Empty;

        public virtual string BeforeTransaction(IServiceModel service, IOperationModel operation) => string.Empty;

        public virtual string BeforeCallToAppLayer(IServiceModel service, IOperationModel operation) => string.Empty;

        public virtual string AfterCallToAppLayer(IServiceModel service, IOperationModel operation) => string.Empty;

        public virtual string AfterTransaction(IServiceModel service, IOperationModel operation) => string.Empty;

        public virtual string OnExceptionCaught(IServiceModel service, IOperationModel operation) => string.Empty;

        public virtual bool HandlesCaughtException() => false;

        public virtual string OnDispose(IServiceModel service) => string.Empty;

        public virtual string ClassMethods(IServiceModel service) => string.Empty;

        public virtual int Priority { get; set; } = 0;
    }
}