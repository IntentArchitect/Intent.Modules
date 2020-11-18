namespace Intent.Modules.ModuleBuilder.Templates.IModSpec
{
    public class FactoryExtensionRegistrationRequiredEvent
    {
        public FactoryExtensionRegistrationRequiredEvent(string modelId, string extensionId)
        {
            ModelId = modelId;
            ExtensionId = extensionId;
        }

        public string ModelId { get; }
        public string ExtensionId { get; set; }
    }
}