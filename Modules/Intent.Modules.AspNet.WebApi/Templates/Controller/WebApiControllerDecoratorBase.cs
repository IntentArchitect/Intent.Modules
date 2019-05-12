﻿using Intent.Modelers.Services.Api;
using Intent.Templates;
using System.Collections.Generic;
using Intent.Metadata.Models;

namespace Intent.Modules.AspNet.WebApi.Templates.Controller
{
    public abstract class WebApiControllerDecoratorBase : ITemplateDecorator, IDeclareUsings
    {
        public virtual IEnumerable<string> DeclareUsings() => new List<string>();

        public virtual string DeclarePrivateVariables(IServiceModel service) => @"";

        public virtual string ConstructorParams(IServiceModel service) => @"";

        public virtual string ConstructorInit(IServiceModel service) => @"";

        public virtual IEnumerable<string> PayloadPropertyDecorators(IOperationParameter parameter) => new string[] {};

        public virtual string BeginOperation(IServiceModel service, IOperation operation) => @"";

        public virtual string BeforeTransaction(IServiceModel service, IOperation operation) => @"";

        public virtual string BeforeCallToAppLayer(IServiceModel service, IOperation operation) => @"";

        public virtual string AfterCallToAppLayer(IServiceModel service, IOperation operation) => @"";

        public virtual string AfterTransaction(IServiceModel service, IOperation operation) => @"";

        public virtual string OnExceptionCaught(IServiceModel service, IOperation operation) => @"";

        public virtual bool HandlesCaughtException() => false;

        public virtual string OnDispose(IServiceModel service) => @"";

        public virtual string ClassMethods(IServiceModel service) => @"";

        public virtual int Priority { get; set; } = 0;
    }
}