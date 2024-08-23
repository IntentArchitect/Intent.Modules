using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class TemplateFileConfig : ITemplateFileConfig
    {
        /// <summary>
        /// Default template configuration.
        /// </summary>
        /// <param name="fileName">
        /// Populates <see cref="FileName"/>.
        /// </param>
        /// <param name="fileExtension">
        /// Populates <see cref="FileExtension"/>.
        /// </param>
        /// <param name="relativeLocation">
        /// Populates <see cref="LocationInProject"/>.
        /// </param>
        /// <param name="overwriteBehaviour">
        /// Populates <see cref="OverwriteBehaviour"/>.
        /// </param>
        /// <param name="codeGenType">
        /// Populates <see cref="CodeGenType"/>.
        /// </param>
        public TemplateFileConfig(
            string fileName,
            string fileExtension,
            string relativeLocation = "",
            OverwriteBehaviour overwriteBehaviour = OverwriteBehaviour.Always,
            string codeGenType = Common.CodeGenType.Basic
        )
        {
            CustomMetadata = new Dictionary<string, string>();
            CodeGenType = codeGenType;
            OverwriteBehaviour = overwriteBehaviour;
            FileName = fileName;
            FileExtension = fileExtension;
            LocationInProject = relativeLocation;
        }

        /// <inheritdoc />
        public virtual string CodeGenType { get; }
        /// <inheritdoc />
        public virtual string FileExtension { get; }
        /// <inheritdoc />
        public virtual OverwriteBehaviour OverwriteBehaviour { get; }
        /// <inheritdoc />
        public virtual string FileName { get; set; }
        /// <inheritdoc />
        public string LocationInProject { get; set; }
        
        /// <inheritdoc />
        IDictionary<string, string> ITemplateFileConfig.CustomMetadata => CustomMetadata;

        /// <inheritdoc />
        public virtual Dictionary<string, string> CustomMetadata { get; }

    }
}
