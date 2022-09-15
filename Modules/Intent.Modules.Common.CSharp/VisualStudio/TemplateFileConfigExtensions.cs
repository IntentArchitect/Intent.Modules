using System;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.CSharp.VisualStudio
{
    /// <summary>
    /// Extension methods for <see cref="ITemplateFileConfig"/>.
    /// </summary>
    public static class TemplateFileConfigExtensions
    {
        private static string NormalizePath(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            // .csproj and solution files use backslashes even on Mac
            value = value.Replace("/", @"\");

            // Replace double occurrences of folder separators with single separator. IE, turn a path like Dev\\Folder to Dev\Folder
            while (value.Contains(@"\\"))
                value = value.Replace(@"\\", @"\");

            return value;
        }

        /// <summary>
        /// Obsolete. Use <see cref="WithFileItemGenerationBehaviour"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public static T WithAlwaysGenerateProjectItem<T>(this T templateFileConfig) where T : ITemplateFileConfig
        {
            return templateFileConfig.WithFileItemGenerationBehaviour(MsBuildFileItemGenerationBehaviour.Always);
        }

        /// <summary>
        /// Adds an attribute to the file item in the MSBuild file.
        /// </summary>
        public static T WithAttribute<T>(this T templateFileConfig, string attributeName, string attributeValue) where T : ITemplateFileConfig
        {
            templateFileConfig.CustomMetadata.Add($"{CustomMetadataKeys.AttributePrefix}{attributeName}", attributeValue);
            return templateFileConfig;
        }

        /// <summary>
        /// Disables auto formatting of the file after code merging.
        /// </summary>
        public static T WithAutoFormatting<T>(this T templateFileConfig, bool enabled) where T : ITemplateFileConfig
        {
            templateFileConfig.CustomMetadata["AutoFormat"] = enabled.ToString();
            return templateFileConfig;
        }

        /// <summary>
        /// Adds a <c>&lt;AutoGen&gt;True&lt;/AutoGen&gt;</c> element to the file item in the MSBuild file.
        /// </summary>
        public static T WithAutoGen<T>(this T templateFileConfig) where T : ITemplateFileConfig
        {
            return templateFileConfig.WithNestedProjectElement("AutoGen", true.ToString());
        }

        /// <summary>
        /// Controls the value of the <c>&lt;CopyToOutputDirectory/ &gt;</c> element for a file item in the MSBuild file.
        /// </summary>
        public static T WithCopyToOutputDirectory<T>(this T templateFileConfig, CopyToOutputDirectory copyToOutputDirectory) where T : ITemplateFileConfig
        {
            return templateFileConfig.WithNestedProjectElement(
                elementName: "CopyToOutputDirectory",
                copyToOutputDirectory switch
                {
                    CopyToOutputDirectory.DoNotCopy => "Never",
                    CopyToOutputDirectory.CopyAlways => "Always",
                    CopyToOutputDirectory.CopyIfNewer => "PreserveNewest",
                    _ => throw new ArgumentOutOfRangeException(nameof(copyToOutputDirectory), copyToOutputDirectory, null)
                });
        }

        /// <summary>
        /// Adds a <c>&lt;DependentUpon/ &gt;</c> element to the file item in the MSBuild file.
        /// </summary>
        public static T WithDependsOn<T>(this T templateFileConfig, ITemplate template) where T : ITemplateFileConfig
        {
            return templateFileConfig.WithDependsOn(template.GetMetadata().GetRelativeFilePath());
        }

        /// <summary>
        /// Adds a <c>&lt;DependentUpon/ &gt;</c> element to the file item in the MSBuild file.
        /// </summary>
        public static T WithDependsOn<T>(this T templateFileConfig, string path) where T : ITemplateFileConfig
        {
            return templateFileConfig.WithNestedProjectElement("DependentUpon", path, isPath: true);
        }

        /// <summary>
        /// Adds a <c>&lt;DesignTime&gt;True&lt;/DesignTime&gt;</c> element to the file item in the MSBuild file.
        /// </summary>
        public static T WithDesignTime<T>(this T templateFileConfig) where T : ITemplateFileConfig
        {
            return templateFileConfig.WithNestedProjectElement("DesignTime", true.ToString());
        }

        /// <summary>
        /// Controls the behaviour of generation of a template's file item entry in its MSBuild file.
        /// </summary>
        public static T WithFileItemGenerationBehaviour<T>(this T templateFileConfig, MsBuildFileItemGenerationBehaviour behaviour) where T : ITemplateFileConfig
        {
            templateFileConfig.CustomMetadata[CustomMetadataKeys.MsBuildFileItemGenerationBehaviour] = behaviour.ToString();
            return templateFileConfig;
        }

        /// <summary>
        /// Indicate that the file in the MSBuild file is never implicitly present in SDK style
        /// projects.
        /// </summary>
        public static T WithItemType<T>(this T templateFileConfig, string itemType) where T : ITemplateFileConfig
        {
            templateFileConfig.CustomMetadata[CustomMetadataKeys.ItemType] = itemType;
            return templateFileConfig;
        }

        /// <summary>
        /// Adds a nested element to the file item in the MSBuild file.
        /// </summary>
        /// <param name="templateFileConfig">The <see cref="ITemplateFileConfig"/> onto which to apply the configuration.</param>
        /// <param name="elementName">The name of element nested element.</param>
        /// <param name="elementValue">The value of the nested element.</param>
        /// <param name="isPath">Whether or not the value is a path, in which case it should be normalized for MS Build such that all folder separators are backslashes.</param>
        /// <returns>The <see cref="ITemplateFileConfig"/> with the configuration applied.</returns>
        public static T WithNestedProjectElement<T>(this T templateFileConfig, string elementName, string elementValue, bool isPath = false) where T : ITemplateFileConfig
        {
            var key = $"{CustomMetadataKeys.ElementPrefix}{elementName}";
            if (elementValue == null)
            {
                if (templateFileConfig.CustomMetadata.ContainsKey(key))
                {
                    templateFileConfig.CustomMetadata.Remove(key);
                }

                return templateFileConfig;
            }

            if (isPath)
            {
                elementValue = NormalizePath(elementValue);
            }

            templateFileConfig.CustomMetadata.Add(key, elementValue);
            return templateFileConfig;
        }

        /// <summary>
        /// Adds the necessary elements to the file item in the MSBuild file for a pre-processed
        /// <c>.tt</c>
        /// </summary>
        public static T WithTextTemplatingFilePreprocessor<T>(this T templateFileConfig) where T : ITemplateFileConfig
        {
            return templateFileConfig
                .WithItemType("None")
                .WithNestedProjectElement("Generator", "TextTemplatingFilePreprocessor")
                .WithNestedProjectElement("LastGenOutput", $"{templateFileConfig.FileName}.cs");
        }
    }
}
