using System.Linq;
using Intent.Modules.Common.TypeScript.Editor;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Weaving
{
    public class TypeScriptFileMerger
    {
        private readonly TypeScriptFile _existingFile;
        private readonly TypeScriptFile _outputFile;

        public TypeScriptFileMerger(TypeScriptFile existingFile, TypeScriptFile outputFile)
        {
            _existingFile = existingFile;
            _outputFile = outputFile;
        }

        public string GetMergedFile()
        {
            if (_existingFile.GetSource().TrimStart().StartsWith("//@IntentIgnoreFile"))
            {
                return _existingFile.GetSource();
            }

            if (_existingFile.GetSource().TrimStart().StartsWith("//@IntentOverwriteFile"))
            {
                return _outputFile.GetSource();
            }

            _existingFile.MergeWith(_outputFile);

            _existingFile.NormalizeImports();

            return _existingFile.GetSource();
        }
    }
}