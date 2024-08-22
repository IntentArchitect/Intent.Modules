using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class TemplateFileConfig : ITemplateFileConfig
    {
        /// <summary>
        /// Template output configuration.
        /// </summary>
        /// <param name="fileName">
        /// The name of the file to be generated.
        /// This can also be used to define "dotfiles" such as ".dockerignore".
        /// </param>
        /// <param name="fileExtension">
        /// The extension of the file, such as ".cs" or ".json". If you want to define a "dotfile" (like ".dockerignore") you will need to define it in <see cref="fileName"/>.
        /// </param>
        /// <param name="relativeLocation">
        /// The relative location to where the template is installed where the output should be generated. Defaults to an empty string.
        /// </param>
        /// <param name="overwriteBehaviour">
        /// Indicates the behaviour for overwriting an existing file. The default is <see cref="OverwriteBehaviour.Always"/>.
        /// </param>
        /// <param name="codeGenType">
        /// Specifies the type of code generation. The default is <see cref="Common.CodeGenType.Basic"/>.
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

        public virtual string CodeGenType { get; }
        public virtual string FileExtension { get; }
        public virtual OverwriteBehaviour OverwriteBehaviour { get; }
        public virtual string FileName { get; set; }
        public string LocationInProject { get; set; }
        IDictionary<string, string> ITemplateFileConfig.CustomMetadata => CustomMetadata;

        public virtual Dictionary<string, string> CustomMetadata { get; }

    }
}
