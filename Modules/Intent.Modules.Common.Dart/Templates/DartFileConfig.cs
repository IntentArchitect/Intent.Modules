using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.Dart.Templates;

/// <summary>
/// Defines configuration for Dart files.
/// </summary>
public class DartFileConfig : TemplateFileConfig
{
    /// <summary>
    /// Creates a new instance of <see cref="DartFileConfig"/>.
    /// </summary>
    public DartFileConfig(
        string className,
        string fileName = null,
        string relativeLocation = null,
        OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
        string codeGenType = Common.CodeGenType.Basic,
        string fileExtension = "dart")
        : base(fileName: fileName ?? className.ToKebabCase(),
            fileExtension: fileExtension,
            relativeLocation: relativeLocation ?? string.Empty,
            overwriteBehaviour: overwriteBehaviour,
            codeGenType: codeGenType)
    {
        if (!string.IsNullOrWhiteSpace(className))
        {
            CustomMetadata["ClassName"] = className;
        }
    }
}