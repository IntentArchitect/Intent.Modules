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
        /// Indicate that the file entry in the MSBuild file should be an <c>EmbeddedResource</c>.
        /// </summary>
        /// <remarks>
        /// This is a convenience method which calls <see cref="WithFileItemGenerationBehaviour{T}"/>,
        /// <see cref="WithItemType{T}"/> with a parameter value of <c>EmbeddedResource</c> as well
        /// as <see cref="WithRemoveItemType{T}"/> with a parameter value of <c>Compile</c> for
        /// <c>.cs</c> and <c>None</c> for all other file types, the <paramref name="removeItemType"/>
        /// parameter can be used to override this.
        /// </remarks>
        public static T AsEmbeddedResource<T>(this T templateFileConfig, string removeItemType = null) where T : ITemplateFileConfig
        {
            removeItemType ??= templateFileConfig.FileExtension == "cs"
                ? "Compile"
                : "None";

            return templateFileConfig
                .WithFileItemGenerationBehaviour(MsBuildFileItemGenerationBehaviour.Always)
                .WithItemType("EmbeddedResource")
                .WithRemoveItemType(removeItemType);
        }

        /// <summary>
        /// Obsolete. Use <see cref="WithFileItemGenerationBehaviour{T}"/> instead.
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
        /// Controls whether or not auto formatting of the file is applied after code merging.
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
        /// Indicate that the file entry in the MSBuild file should have the provided <paramref name="itemType"/>, ie,
        /// <c>&lt;&lt;itemType&gt; Update="&lt;fileName&gt;"/&gt;</c> should be added to the .csproj file. If
        /// <see cref="WithRemoveItemType{T}"/> has been used, then <c>&lt;&lt;itemType&gt; Include="&lt;fileName&gt;"/&gt;</c>
        /// will be generated instead.
        /// </summary>
        public static T WithItemType<T>(this T templateFileConfig, string itemType) where T : ITemplateFileConfig
        {
            return templateFileConfig.WithItemType(itemType, wasAddedImplicitly: true);
        }

        /// <summary>
        /// Indicate that the file entry in the MSBuild file should have the provided <paramref name="itemType"/>, ie,
        /// <c>&lt;&lt;itemType&gt; Update="&lt;fileName&gt;"/&gt;</c> should be added to the .csproj file. If
        /// <see cref="WithRemoveItemType{T}"/> has been used or <paramref name="wasAddedImplicitly"/> is
        /// <see langword="false"/>, then <c>&lt;&lt;itemType&gt; Include="&lt;fileName&gt;"/&gt;</c> will be generated
        /// instead.
        /// </summary>
        public static T WithItemType<T>(this T templateFileConfig, string itemType, bool wasAddedImplicitly) where T : ITemplateFileConfig
        {
            templateFileConfig.CustomMetadata[CustomMetadataKeys.ItemType] = itemType;
            templateFileConfig.CustomMetadata[CustomMetadataKeys.WasAddedImplicitly] = wasAddedImplicitly.ToString();
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
        /// Indicate that the file entry in the MSBuild file should have a "Remove" entry added for it, ie,
        /// that <c>&lt;&lt;itemType&gt; Remove="&lt;filename&gt;" /&gt;</c> should be added to the .csproj file.
        /// </summary>
        public static T WithRemoveItemType<T>(this T templateFileConfig, string itemType) where T : ITemplateFileConfig
        {
            templateFileConfig.CustomMetadata[CustomMetadataKeys.RemoveItemType] = itemType;
            return templateFileConfig;
        }

        /// <summary>
        /// Adds the necessary elements to the file item in the MSBuild file for a pre-processed
        /// <c>.tt</c> file.
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
