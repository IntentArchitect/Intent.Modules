using System;
using Intent.Modules.Common.CSharp.VisualStudio;
using Intent.Modules.Common.Templates;
using Intent.SdkEvolutionHelpers;
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
        /// Sets the C# file configuration.
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
            ApplyNamespaceFormatting = true;

            this.WithItemType("Compile");
        }

        /// <summary>
        /// Whether or not to automatically apply formatting to C# files.
        /// </summary>
        public bool AutoFormat
        {
            get => bool.TryParse(CustomMetadata[nameof(AutoFormat)], out var parsed) && parsed;
            set => CustomMetadata[nameof(AutoFormat)] = value.ToString();
        }

        /// <summary>
        /// Disables the automatic formatting of this C# by the Roslyn Weaving system.
        /// </summary>
        /// <returns></returns>
        public CSharpFileConfig DisableAutoFormat()
        {
            AutoFormat = false;
            return this;
        }

        /// <summary>
        /// The primary class name of this file.
        /// </summary>
        public string ClassName
        {
            get => CustomMetadata["ClassName"];
            set => CustomMetadata["ClassName"] = value;
        }

        /// <inheritdoc />
        public string Namespace
        {
            get => CustomMetadata["Namespace"];
            set => CustomMetadata["Namespace"] = value;
        }

        /// <summary>
        /// Whether or not to apply formatting (such as PascalCasing) to namespaces. 
        /// </summary>
        [FixFor_Version4] // See if we can get rid of this, not sure what it's even being used for.
        public bool ApplyNamespaceFormatting
        {
            get => bool.TryParse(CustomMetadata[nameof(ApplyNamespaceFormatting)], out var parsed) && parsed;
            set => CustomMetadata[nameof(ApplyNamespaceFormatting)] = value.ToString();
        }

        /// <summary>
        /// Sets the default Roslyn Weaver Tag Mode for the template to be <c>Explicit</c>.
        /// </summary>
        public CSharpFileConfig IntentTagModeExplicit()
        {
            CustomMetadata["RoslynWeaverTagMode"] = "Explicit";
            return this;
        }

        /// <summary>
        /// Sets the default Roslyn Weaver Tag Mode for the template to be <c>Implicit</c>.
        /// </summary>
        public CSharpFileConfig IntentTagModeImplicit()
        {
            CustomMetadata["RoslynWeaverTagMode"] = "Implicit";
            return this;
        }
    }
}
