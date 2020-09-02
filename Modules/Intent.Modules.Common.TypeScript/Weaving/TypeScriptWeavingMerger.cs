using Intent.Modules.Common.TypeScript.Editor;

namespace Intent.Modules.Common.TypeScript.Weaving
{
    public class TypeScriptWeavingMerger
    {
        public TypeScriptWeavingMerger()
        {
        }

        public string Merge(string existingContent, string outputContent)
        {
            return Merge(new TypeScriptFile(existingContent), outputContent);
        }

        public string Merge(TypeScriptFile existingFile, string outputContent)
        {
            var merger = new TypeScriptFileMerger(existingFile, new TypeScriptFile(outputContent));
            return merger.GetMergedFile();
        }
    }
}