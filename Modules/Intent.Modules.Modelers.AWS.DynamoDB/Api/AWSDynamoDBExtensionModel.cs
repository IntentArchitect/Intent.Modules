using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiElementExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.DynamoDB.Api
{
    /// <summary>
    /// Intentionally ignored so that no runtime dependency on CDK is required.
    /// </summary>
    [IntentManaged(Mode.Ignore)]
    internal class AWSDynamoDBExtensionModel
    {
    }
}