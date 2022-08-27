namespace Intent.Modules.Common
{
    public interface ITemplateBeforeExecutionHook
    {
        void BeforeTemplateExecution();
    }

    public interface IAfterTemplateRegistrationExecutionHook
    {
        void AfterTemplateRegistration();
    }
}