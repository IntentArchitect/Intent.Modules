using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class DefaultFileMetaData : ITemplateFileConfig
    {

        public DefaultFileMetaData(
            OverwriteBehaviour overwriteBehaviour,
            string codeGenType,
            string fileName,
            string fileExtension,
            string defaultLocationInProject
            )
        {
            CustomMetaData = new Dictionary<string, string>();
            CodeGenType = codeGenType;
            OverwriteBehaviour = overwriteBehaviour;
            FileName = fileName;
            FileExtension = fileExtension;
            DefaultLocationInProject = defaultLocationInProject;
        }

        public virtual string CodeGenType { get; }
        public virtual string FileExtension { get; }
        public virtual OverwriteBehaviour OverwriteBehaviour { get; }
        public virtual string FileName { get; set; }
        public string LocationInProject { get; set; }
        IDictionary<string, string> ITemplateFileConfig.CustomMetaData => CustomMetaData;

        public virtual string DefaultLocationInProject { get; }
        public virtual Dictionary<string, string> CustomMetaData { get; }

    }
}
