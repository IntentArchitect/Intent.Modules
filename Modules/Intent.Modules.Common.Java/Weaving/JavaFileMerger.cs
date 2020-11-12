using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Intent.Modules.Common.Java.Editor;
using Intent.Utils;

namespace Intent.Modules.Common.Java.Weaving
{
    public class JavaFileMerger
    {
        private readonly JavaFile _existingFile;
        private readonly JavaFile _outputFile;

        public JavaFileMerger(JavaFile existingFile, JavaFile outputFile)
        {
            _existingFile = existingFile;
            _outputFile = outputFile;
        }

        public string GetMergedFile()
        {
            if (_outputFile.Package != null)
            {
                _existingFile.SetPackage(_outputFile.Package);
            }

            foreach (var import in _outputFile.Imports)
            {
                if (!_existingFile.ImportExists(import))
                {
                    _existingFile.AddImport(import);
                }
            }

            _existingFile.MergeWith(_outputFile);

            return _existingFile.GetSource();
        }
    }
}