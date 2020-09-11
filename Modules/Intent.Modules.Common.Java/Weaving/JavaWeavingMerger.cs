using Intent.Modules.Common.Java.Editor;
using Intent.Modules.Common.Java.Editor.Parser;

namespace Intent.Modules.Common.Java.Weaving
{
    public class JavaWeavingMerger
    {
        public JavaWeavingMerger()
        {
        }

        public string Merge(string existingContent, string outputContent)
        {
            return Merge(JavaASTParser.Parse(existingContent), outputContent);
        }

        public string Merge(JavaFile existingFile, string outputContent)
        {
            var merger = new JavaFileMerger(existingFile, JavaASTParser.Parse(outputContent));
            return merger.GetMergedFile().TrimEnd();
        }
    }
}