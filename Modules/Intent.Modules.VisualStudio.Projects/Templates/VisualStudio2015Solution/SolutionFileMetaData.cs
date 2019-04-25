using Intent.Templates
using System;
using System.Collections.Generic;
using System.IO;

namespace Intent.Modules.VisualStudio.Projects.Templates.VisualStudio2015Solution
{
    public class SolutionFileMetaData : IFileMetadata
    {
        private readonly string _fileLocation;

        public SolutionFileMetaData(
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
            CustomMetaData = new Dictionary<string, string>();
        }

        public string CodeGenType { get; }
        public string OutputType { get; }
        public OverwriteBehaviour OverwriteBehaviour { get; }
        public string FileName { get; }
        public string FileExtension => "sln";
        public string DefaultLocationInProject => "";
        public string DependsUpon => null;
        public IDictionary<string, string> CustomMetaData { get; }

        public string GetFullLocationPath()
        {
            return Path.GetFullPath(_fileLocation);
        }


        public string GetRelativeFilePath()
        {
            throw new NotImplementedException();
        }
    }
}
