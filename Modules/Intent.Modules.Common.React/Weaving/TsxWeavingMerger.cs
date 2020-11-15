using Intent.Code.Weaving.TypeScript;
using Intent.Code.Weaving.TypeScript.Editor;
using Intent.Modules.Common.React.Editor;

namespace Intent.Modules.Common.React.Weaving
{
    public class TsxWeavingMerger
    {
        public TsxWeavingMerger()
        {
        }

        public string Merge(string existingContent, string outputContent)
        {
            return Merge(new TsxFileEditor(existingContent).File, outputContent);
        }

        public string Merge(TypeScriptFile existingFile, string outputContent)
        {
            var merger = new TypeScriptFileMerger(existingFile, new TsxFileEditor(outputContent).File);
            return merger.GetMergedFile();
        }
    }
}