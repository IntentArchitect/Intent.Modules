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
            string dependsUpon = null)
            : base(fileName ?? className, fileExtension, relativeLocation, overwriteBehaviour, "RoslynWeave")
        {
            CustomMetadata["ClassName"] = className ?? throw new ArgumentNullException(nameof(className));

            if (!string.IsNullOrWhiteSpace(@namespace))
            {
                CustomMetadata["Namespace"] = @namespace;
            }

            if (!string.IsNullOrWhiteSpace(dependsUpon))
            {
                CustomMetadata["Depends On"] = dependsUpon;
            }

            AutoFormat = true;
        }

        /// <summary>
        /// C# styling automatically formatted
        /// </summary>
        public bool AutoFormat
        {
            get
            {
                var val = CustomMetadata["AutoFormat"];
                if (string.IsNullOrEmpty(val)) { return false; }
                return bool.Parse(val);
            }
            set 
            {
                CustomMetadata["AutoFormat"] = value.ToString();
            }
        }
    }
}
