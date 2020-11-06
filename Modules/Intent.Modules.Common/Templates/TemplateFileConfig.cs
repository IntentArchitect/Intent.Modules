using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class TemplateFileConfig : ITemplateFileConfig
    {
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
