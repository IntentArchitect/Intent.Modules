using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: DefaultIntentManaged(Mode.Ignore, Targets = Targets.Usings)]
[assembly: IntentTemplate("Intent.ModuleBuilder.Templates.Api.ApiPackageExtensionModel", Version = "1.0")]

namespace Intent.Modelers.AWS.Lambda.Api
{
    /// <summary>
    /// Intentionally ignored so that no runtime dependency on the module builder is required.
    /// </summary>
    [IntentManaged(Mode.Ignore)]
    internal class ModuleBuilderAWSLambdaPackageExtensionsModel
    {
    }
}