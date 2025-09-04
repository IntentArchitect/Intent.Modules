#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using Intent.Engine;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;

namespace Intent.Modules.Common.Templates.StaticContent
{
    /// <summary>
    /// Outputs the contents of the file as per the path provided in 'sourcePath' constructor
    /// parameter.
    /// </summary>
    public class StaticContentTemplate : IntentTemplateBase
    {
        private readonly string _sourcePath;
        private readonly string? _relativeOutputPathPrefix;
        private readonly OverwriteBehaviour _overwriteBehaviour;
        private readonly Func<ITemplateFileConfig, StaticContentTemplate, ITemplateFileConfig> _fileConfigConfigurationUpdater;
        private readonly IReadOnlyDictionary<string, string> _replacements;
        private readonly string _relativeOutputPath;

        /// <summary>
        /// Obsolete. Use <see cref="StaticContentTemplate(string,string,string,string,IOutputTarget,IReadOnlyDictionary{string,string},OverwriteBehaviour,Func{ITemplateFileConfig,ITemplateFileConfig})"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public StaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements)
            : this(
                sourcePath: sourcePath,
                relativeOutputPath: relativeOutputPath,
                relativeOutputPathPrefix: null,
                templateId: templateId,
                outputTarget: outputTarget,
                replacements: replacements,
                overwriteBehaviour: OverwriteBehaviour.OverwriteDisabled,
                fileConfigConfigurationUpdater: (fileConfig, _) => fileConfig)
        {
        }

        /// <summary>
        /// Obsolete. Use <see cref="StaticContentTemplate(string,string,string,string,IOutputTarget,IReadOnlyDictionary{string,string},OverwriteBehaviour,Func{ITemplateFileConfig,ITemplateFileConfig})"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public StaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour)
            : this(
                sourcePath: sourcePath,
                relativeOutputPath: relativeOutputPath,
                relativeOutputPathPrefix: null,
                templateId: templateId,
                outputTarget: outputTarget,
                replacements: replacements,
                overwriteBehaviour: overwriteBehaviour,
                fileConfigConfigurationUpdater: (fileConfig, _) => fileConfig)
        {
        }

        /// <summary>
        /// Obsolete. Use <see cref="StaticContentTemplate(string,string,string,string,IOutputTarget,IReadOnlyDictionary{string,string},OverwriteBehaviour,Func{ITemplateFileConfig,ITemplateFileConfig})"/> instead.
        /// </summary>
        [Obsolete(WillBeRemovedIn.Version4)]
        public StaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string relativeOutputPathPrefix,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour)
            : this(
                sourcePath: sourcePath,
                relativeOutputPath: relativeOutputPath,
                relativeOutputPathPrefix: relativeOutputPathPrefix,
                templateId: templateId,
                outputTarget: outputTarget,
                replacements: replacements,
                overwriteBehaviour: overwriteBehaviour,
                fileConfigConfigurationUpdater: (fileConfig, _) => fileConfig)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="StaticContentTemplate"/>.
        /// </summary>
        public StaticContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string? relativeOutputPathPrefix,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour,
            Func<ITemplateFileConfig, StaticContentTemplate, ITemplateFileConfig> fileConfigConfigurationUpdater) : base(templateId, outputTarget)
        {
            _sourcePath = sourcePath;
            _overwriteBehaviour = overwriteBehaviour;
            _fileConfigConfigurationUpdater = fileConfigConfigurationUpdater;
            _replacements = replacements ?? new Dictionary<string, string>
            {
                ["ApplicationName"] = outputTarget.ApplicationName(),
                ["ApplicationNameAllLowerCase"] = outputTarget.ApplicationName().ToLowerInvariant()
            };

            if (!string.IsNullOrWhiteSpace(relativeOutputPathPrefix))
            {
                relativeOutputPath = Path.Join(relativeOutputPathPrefix, relativeOutputPath);
                _relativeOutputPathPrefix = relativeOutputPathPrefix;
            }

            _relativeOutputPath = relativeOutputPath.NormalizePath();

        }

        /// <inheritdoc />
        public override string TransformText()
        {
            var result = File.ReadAllText(_sourcePath);

            foreach (var (searchFor, replaceWith) in _replacements)
            {
                result = result.Replace($"<#= {searchFor} #>", replaceWith);
            }

            return result;
        }

        /// <inheritdoc />
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            ITemplateFileConfig config = new TemplateFileConfig(
                fileName: Path.GetFileNameWithoutExtension(_relativeOutputPath),
                fileExtension: Path.GetExtension(_relativeOutputPath).TrimStart('.') ?? string.Empty,
                relativeLocation: Path.GetDirectoryName(_relativeOutputPath),
                overwriteBehaviour: _overwriteBehaviour);

            if (_relativeOutputPathPrefix != null)
            {
                config.CustomMetadata.Add("RelativeOutputPathPrefix", _relativeOutputPathPrefix);
            }

            config = _fileConfigConfigurationUpdater(config, this);

            return config;
        }

        /// <inheritdoc />
        public override string GetCorrelationId()
        {
            return $"{Id}#{_relativeOutputPath}";
        }
    }
}