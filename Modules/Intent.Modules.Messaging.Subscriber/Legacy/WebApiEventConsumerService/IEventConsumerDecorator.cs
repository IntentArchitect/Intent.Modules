using Intent.SoftwareFactory.Templates;

namespace Intent.Packages.Messaging.Subscriber.Legacy.WebApiEventConsumerService
{
    public interface IEventConsumerDecorator : ITemplateDecorator, IPriorityDecorator, IDeclareUsings
    {
        string DeclarePrivateVariables();
        string ConstructorParams();
        string ConstructorInit();
        string BeginOperation();
        string BeforeTransaction();
        string BeforeCallToAppLayer();
        string AfterCallToAppLayer();
        string AfterTransaction();
        string OnExceptionCaught();
        bool HandlesCaughtException();
        string ClassMethods();
    }
}