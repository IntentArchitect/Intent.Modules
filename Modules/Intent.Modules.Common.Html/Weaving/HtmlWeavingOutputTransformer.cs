using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Intent.Code.Weaving.Html;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Html.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.Html.Weaving
{
    public class HtmlWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.Common.Html.OutputWeaver";

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is IHtmlFileMerge htmlFileMerge))
            {
                return;
            }

            var existingFile = htmlFileMerge.GetExistingFile();

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
