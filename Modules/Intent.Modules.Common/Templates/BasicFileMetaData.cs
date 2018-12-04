using System.Collections.Generic;
using Intent.SoftwareFactory.Templates;

namespace Intent.Modules.Common.Templates
{
    public class BasicFileMetaData : IFileMetaData
    {

        public BasicFileMetaData(
            string fileName,
            string fileExtension,
            OverwriteBehaviour overwriteBehaviour,
            string codeGenType,
            string fullLocationPath,
            string relativeFilePath,
            string defaultLocationInProject,
            string dependsUpon
            )
        {
            DefaultLocationInProject = defaultLocationInProject;
            DependsUpon = dependsUpon;
            if (fileExtension.StartsWith("."))
            {
                FileExtension = fileExtension.Substring(1);
            }
            else
            {
                FileExtension = fileExtension;
            }
            FileName = fileName;
            CodeGenType = codeGenType;
            OverwriteBehaviour = overwriteBehaviour;
            FullLocationPath = fullLocationPath;
            RelativeFilePath = relativeFilePath;
            CustomMetaData = new Dictionary<string, string>();
        }

        public string CodeGenType { get; }
        public string DefaultLocationInProject { get; }
        public string DependsUpon { get; }
        public string FileExtension { get; }
        public string FileName { get; }
        public OverwriteBehaviour OverwriteBehaviour { get; }
        public string FullLocationPath { get; }
        public string RelativeFilePath { get; }
        public IDictionary<string, string> CustomMetaData { get; }

        public string GetFullLocationPath()
        {
            return FullLocationPath;
        }

        public string GetRelativeFilePath()
        {
            return RelativeFilePath;
        }
    }

}
