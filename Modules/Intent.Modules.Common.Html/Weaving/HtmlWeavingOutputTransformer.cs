using System;
using Intent.Code.Weaving.Html;
using Intent.Code.Weaving.Html.Editor;
using Intent.Engine;
using Intent.Modules.Common.Html.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.Html.Weaving
{
    /// <summary>
    /// An <see cref="ITransformOutput"/> which will weave changes between an existing file and
    /// the output of a template respecting weaving instructions in attributes in the files.
    /// </summary>
    public class HtmlWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.Common.Html.OutputWeaver";

        public bool CanTransform(IOutputFile output)
        {
            return output.Template is IHtmlFileMerge;
        }

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is IHtmlFileMerge))
            {
                throw new InvalidOperationException($"Cannot transform outputs where the template does not derive from {nameof(IHtmlFileMerge)}");
            }

            var existingFile = output.TargetFileExists()
                ? new HtmlFile(output.GetExistingFileContent())
                : null;

            if (existingFile == null)
            {
                return;
            }

            try
            {
                var merger = new HtmlWeavingMerger();

                var newContent = merger.Merge(existingFile, output.Content);

                output.ChangeContent(newContent);
            }
            catch (Exception e)
            {
                output.ChangeContent(existingFile.GetSource());

                Logging.Log.Failure($"Error while weaving Html file: {output.FileMetadata.GetRelativeFilePath()}");
                Logging.Log.Failure(e.ToString());
            }
        }
    }
}
