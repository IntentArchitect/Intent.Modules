namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class DecoratorRegistrationRequiredEvent
    {
        public DecoratorRegistrationRequiredEvent(string modelId, string decoratorId)
        {
            ModelId = modelId;
            DecoratorId = decoratorId;
        }

        public string ModelId { get; }
        public string DecoratorId { get; set; }
    }
}