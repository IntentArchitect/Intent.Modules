using Intent.Engine;
using Intent.SdkEvolutionHelpers;
using Intent.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Intent.Modules.Common.Templates.StaticContent
{
    internal class StaticBinaryContentTemplate : IntentBinaryTemplateBase
    {
        private readonly string _sourcePath;
        private readonly OverwriteBehaviour _overwriteBehaviour;
        private readonly string _relativeOutputPath;


        /// <summary>
        /// Creates a new instance of <see cref="StaticBinaryContentTemplate"/>.
        /// </summary>
        public StaticBinaryContentTemplate(
            string sourcePath,
            string relativeOutputPath,
            string templateId,
            IOutputTarget outputTarget,
            IReadOnlyDictionary<string, string> replacements,
            OverwriteBehaviour overwriteBehaviour) : base(templateId, outputTarget)
        {
            _sourcePath = sourcePath;
            _overwriteBehaviour = overwriteBehaviour;
            _relativeOutputPath = relativeOutputPath.NormalizePath();
        }

        public override byte[] RunBinaryTemplate()
        {
            return File.ReadAllBytes(_sourcePath);
        }

        /// <inheritdoc />
        public override ITemplateFileConfig GetTemplateFileConfig()
        {
            return new TemplateFileConfig(
                fileName: Path.GetFileNameWithoutExtension(_relativeOutputPath),
                fileExtension: Path.GetExtension(_relativeOutputPath)?.TrimStart('.') ?? string.Empty,
                relativeLocation: Path.GetDirectoryName(_relativeOutputPath),
                overwriteBehaviour: _overwriteBehaviour);
        }

        /// <inheritdoc />
        public override string GetCorrelationId()
        {
            return $"{Id}#{_relativeOutputPath}";
        }
    }
}