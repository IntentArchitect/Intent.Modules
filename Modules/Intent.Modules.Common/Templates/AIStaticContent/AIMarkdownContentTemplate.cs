using Intent.Engine;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Intent.Modules.Common.Templates.AIStaticContent
{
    internal class AIMarkdownContentTemplate : IntentTemplateBase
    {
        private readonly string _sourcePath;
        private readonly string _relativeOutputPathPrefix;
        private readonly OverwriteBehaviour _overwriteBehaviour;
        private readonly IReadOnlyDictionary<string, string> _replacements;
        private readonly string _relativeOutputPath;
        private string? _currentContent;
        private bool _dontOverrideContent = false;

        /// <summary>
        /// Creates a new instance of <see cref="AIMarkdownContentTemplate"/>.
        /// </summary>
        public AIMarkdownContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string relativeOutputPathPrefix,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour) : base(templateId, outputTarget)
        {
            _sourcePath = sourcePath;
            _overwriteBehaviour = overwriteBehaviour;
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

        public override void AfterTemplateRegistration()
        {
            base.AfterTemplateRegistration();
            string? existingFile = this.GetExistingFilePath();
            if (existingFile != null && File.Exists(existingFile))
            {
                var existingContent = File.ReadAllText(existingFile);
                if (MarkdownContentHash.HasChanged(existingContent))
                {
                    _currentContent = existingContent;
                    _dontOverrideContent = true;
                }
            }
        }

        public override bool CanRunTemplate()
        {
            var result = base.CanRunTemplate();
            return result;
        }

        /// <inheritdoc />
        public override string TransformText()
        {
            if (_dontOverrideContent && _currentContent != null)
            {
                return _currentContent;
            }

            var result = File.ReadAllText(_sourcePath);

            foreach (var (searchFor, replaceWith) in _replacements)
            {
                result = result.Replace($"<#= {searchFor} #>", replaceWith);
            }

            result = MarkdownContentHash.AddOrUpdateContentHash(result);

            return result;
        }

        /// <inheritdoc />
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            var config = new TemplateFileConfig(
                fileName: Path.GetFileNameWithoutExtension(_relativeOutputPath),
                fileExtension: Path.GetExtension(_relativeOutputPath)?.TrimStart('.') ?? string.Empty,
                relativeLocation: Path.GetDirectoryName(_relativeOutputPath),
                overwriteBehaviour: _overwriteBehaviour);

            if (_relativeOutputPathPrefix != null)
            {
                config.CustomMetadata.Add("RelativeOutputPathPrefix", _relativeOutputPathPrefix);
            }

            return config;
        }

        /// <inheritdoc />
        public override string GetCorrelationId()
        {
            return $"{Id}#{_relativeOutputPath}";
        }
    }
}
