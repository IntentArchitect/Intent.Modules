using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Html.Editor;

namespace Intent.Modules.Common.Html.Weaving
{
    public class HtmlFileMerger
    {
        private readonly HtmlFile _existingFile;
        private readonly HtmlFile _outputFile;

        public HtmlFileMerger(HtmlFile existingFile, HtmlFile outputFile)
        {
            _existingFile = existingFile;
            _outputFile = outputFile;
        }

        public string GetMergedFile()
        {
            if (string.IsNullOrWhiteSpace(_existingFile.Node.OuterHtml))
            {
                return _outputFile.GetSource();
            }
            _existingFile.MergeWith(_outputFile);

            return _existingFile.GetSource();
        }
    }
}