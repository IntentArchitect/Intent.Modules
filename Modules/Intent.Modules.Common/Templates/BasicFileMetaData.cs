using System.Collections.Generic;
using Intent.Templates;

namespace Intent.Modules.Common.Templates
{
    public class BasicFileMetadata : IFileMetadata
    {

        public BasicFileMetadata(
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
            LocationInProject = defaultLocationInProject;
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
            CustomMetadata = new Dictionary<string, string>();
        }

        public string CodeGenType { get; }
        public string DependsUpon { get; }
        public string FileExtension { get; }
        public string FileName { get; set; }
        public string LocationInProject { get; set; }
        public OverwriteBehaviour OverwriteBehaviour { get; }
        public string FullLocationPath { get; }
        public string RelativeFilePath { get; }
        public IDictionary<string, string> CustomMetadata { get; }

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
