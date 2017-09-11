using System.Collections.Generic;
using Intent.Modules.WebApi.Legacy.Controller;
using Intent.SoftwareFactory.MetaModels.Class;
using Intent.SoftwareFactory.MetaModels.Service;

namespace Intent.Modules.WebApi.Legacy
{
    public abstract class BaseDistributionDecorator : IDistributionDecorator
    {
        public virtual IEnumerable<string> DeclareUsings(ServiceModel service) => new List<string>();

        public virtual string DeclarePrivateVariables(ServiceModel service) => @"";

        public virtual string ConstructorParams(ServiceModel service) => @"";

        public virtual string ConstructorInit(ServiceModel service) => @"";

        public virtual IEnumerable<string> PayloadPropertyDecorators(ParameterModel parameter) => new string[] {};

        public virtual string BeginOperation(ServiceModel service, ServiceOperationModel operation) => @"";

        public virtual string BeforeTransaction(ServiceModel service, ServiceOperationModel operation) => @"";

        public virtual string BeforeCallToAppLayer(ServiceModel service, ServiceOperationModel operation) => @"";

        public virtual string AfterCallToAppLayer(ServiceModel service, ServiceOperationModel operation) => @"";

        public virtual string AfterTransaction(ServiceModel service, ServiceOperationModel operation) => @"";

        public virtual string OnExceptionCaught(ServiceModel service, ServiceOperationModel operation) => @"";

        public virtual bool HandlesCaughtException() => false;

        public virtual string ClassMethods(ServiceModel service) => @"";

        public virtual int Priority { get; set; } = 0;
    }
}