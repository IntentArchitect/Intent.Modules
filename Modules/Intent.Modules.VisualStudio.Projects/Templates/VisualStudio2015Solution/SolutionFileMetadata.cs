using Intent.Templates;
using System;
using System.Collections.Generic;
using System.IO;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    public class SolutionFileMetadata : IFileMetadata, ITemplateFileConfig
    {
        private readonly string _fileLocation;

        public SolutionFileMetadata(
            string outputType,
            OverwriteBehaviour overwriteBehaviour,
            string codeGenType,
            string fileName,
            string fileLocation)
        {
            _fileLocation = fileLocation;
            OutputType = outputType;
            OverwriteBehaviour = overwriteBehaviour;
            FileName = fileName;
            CodeGenType = codeGenType;
            CustomMetadata = new Dictionary<string, string>();
        }

        public string CodeGenType { get; }
        public string OutputType { get; }
        public OverwriteBehaviour OverwriteBehaviour { get; }
        public string FileName { get; set; }
        public string LocationInProject { get; set; } = "";
        public string FileExtension => "sln";
        public string DependsUpon => null;
        public IDictionary<string, string> CustomMetadata { get; }

        public string GetFullLocationPath()
        {
            return Path.GetFullPath(_fileLocation).Replace(@"\", "/");
        }

        public string GetRelativeFilePath()
        {
            throw new NotImplementedException();
        }
    }
}
