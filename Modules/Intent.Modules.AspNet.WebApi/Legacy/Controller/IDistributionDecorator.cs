using Intent.SoftwareFactory.MetaModels.Class;
using Intent.SoftwareFactory.MetaModels.Service;
using Intent.SoftwareFactory.Templates;
using System.Collections.Generic;

namespace Intent.Modules.AspNet.WebApi.Legacy.Controller
{
    public interface IDistributionDecorator : ITemplateDecorator, IPriorityDecorator
    {
        IEnumerable<string> DeclareUsings(ServiceModel service);
        string DeclarePrivateVariables(ServiceModel service);
        string ConstructorParams(ServiceModel service);
        string ConstructorInit(ServiceModel service);
        IEnumerable<string> PayloadPropertyDecorators(ParameterModel parameter);
        string BeginOperation(ServiceModel service, ServiceOperationModel operation);
        string BeforeTransaction(ServiceModel service, ServiceOperationModel operation);
        string BeforeCallToAppLayer(ServiceModel service, ServiceOperationModel operation);
        string AfterCallToAppLayer(ServiceModel service, ServiceOperationModel operation);
        string AfterTransaction(ServiceModel service, ServiceOperationModel operation);
        string OnExceptionCaught(ServiceModel service, ServiceOperationModel operation);
        bool HandlesCaughtException();
        string ClassMethods(ServiceModel service);
    }
}