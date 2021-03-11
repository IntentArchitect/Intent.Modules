using System;
using Intent.Modules.Common.Templates;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.Templates
{
    /// <summary>
    /// Specialization of <see cref="TemplateFileConfig"/> for setting
    /// metadata specific to C# templates.
    /// </summary>
    public class CSharpFileConfig : TemplateFileConfig
    {
        /// <summary>
        /// Sets the C# file configuration
        /// </summary>
        public CSharpFileConfig(
            string className,
            string @namespace,
            string relativeLocation = "",
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string fileName = null,
            string fileExtension = "cs",
            string dependsUpon = null) : this(
                className: className,
                @namespace: @namespace,
                relativeLocation: relativeLocation,
                overwriteBehaviour: overwriteBehaviour,
                fileName: fileName,
                fileExtension: fileExtension,
                dependsUpon: dependsUpon,
                autoFormat: true)
        {
            // This overload is for backwards compatibility for templates which were compiled
            // against this method signature. Even though the below overload's new 'autoFormat'
            // parameter is optional, C# assemblies compiled against it before its introduction
            // don't see it as the same method and throw a method not found exception when trying
            // to call it.
        }

        /// <summary>
        /// Sets the C# file configuration
        /// </summary>
        public CSharpFileConfig(
            string className,
            string @namespace,
            string relativeLocation = "",
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string fileName = null,
            string fileExtension = "cs",
            string dependsUpon = null,
            bool autoFormat = true)
            : base(fileName ?? className, fileExtension, relativeLocation, overwriteBehaviour, "RoslynWeave")
        {
            CustomMetadata["ClassName"] = className ?? throw new ArgumentNullException(nameof(className));
            CustomMetadata["AutoFormat"] = autoFormat.ToString();
            
            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                CustomMetadata["Namespace"] = @namespace;
            }
            
            if (!string.IsNullOrWhiteSpace(dependsUpon))
            {
                CustomMetadata["Depends On"] = dependsUpon;
            }
        }
    }
}