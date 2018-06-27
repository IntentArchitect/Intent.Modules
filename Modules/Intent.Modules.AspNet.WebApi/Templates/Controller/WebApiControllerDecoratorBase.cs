using Intent.MetaModel.Service;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Templates.Controller
{
    public abstract class WebApiControllerDecoratorBase : ITemplateDecorator, IPriorityDecorator
    {
        public virtual IEnumerable<string> DeclareUsings(IServiceModel service) => new List<string>();

        public virtual string DeclarePrivateVariables(IServiceModel service) => @"";

        public virtual string ConstructorParams(IServiceModel service) => @"";

        public virtual string ConstructorInit(IServiceModel service) => @"";

        public virtual IEnumerable<string> PayloadPropertyDecorators(IOperationParameterModel parameter) => new string[] {};

        public virtual string BeginOperation(IServiceModel service, IOperationModel operation) => @"";

        public virtual string BeforeTransaction(IServiceModel service, IOperationModel operation) => @"";

        public virtual string BeforeCallToAppLayer(IServiceModel service, IOperationModel operation) => @"";

        public virtual string AfterCallToAppLayer(IServiceModel service, IOperationModel operation) => @"";

        public virtual string AfterTransaction(IServiceModel service, IOperationModel operation) => @"";

        public virtual string OnExceptionCaught(IServiceModel service, IOperationModel operation) => @"";

        public virtual bool HandlesCaughtException() => false;

        public virtual string OnDispose(IServiceModel service) => @"";

        public virtual string ClassMethods(IServiceModel service) => @"";

        public virtual int Priority { get; set; } = 0;
    }
}