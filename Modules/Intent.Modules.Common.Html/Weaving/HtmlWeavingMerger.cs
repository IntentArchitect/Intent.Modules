using Intent.Modules.Common.Html.Editor;

namespace Intent.Modules.Common.Html.Weaving
{
    public class HtmlWeavingMerger
    {
        public HtmlWeavingMerger()
        {
        }

        public string Merge(string existingContent, string outputContent)
        {
            return Merge(new HtmlFile(existingContent), outputContent);
        }

        public string Merge(HtmlFile existingFile, string outputContent)
        {
            var merger = new HtmlFileMerger(existingFile, new HtmlFile(outputContent));
            return merger.GetMergedFile();
        }
    }
}