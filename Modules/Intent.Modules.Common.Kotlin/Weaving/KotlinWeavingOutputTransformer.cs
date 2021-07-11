using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Intent.Code.Weaving.Kotlin;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.Kotlin.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.Kotlin.Weaving
{
    public class KotlinWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.Common.Kotlin.OutputWeaver";

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is IKotlinMerged KotlinMerged))
            {
                return;
            }

            var existingFile = KotlinMerged.GetExistingFile();

            if (existingFile == null)
            {
                return;
            }

            try
            {
                var merger = new KotlinWeavingMerger();

                var sw = Stopwatch.StartNew();
                var newContent = merger.Merge(existingFile, output.Content);
                Logging.Log.Debug($"Merged file: {output.FileMetadata.GetFilePath()} ({sw.ElapsedMilliseconds}ms)");

                output.ChangeContent(newContent);
            }
            catch (Exception e)
            {
                output.ChangeContent(existingFile.GetSource());

                Logging.Log.Failure($"Error while weaving Kotlin file: {output.FileMetadata.GetFilePath()}");
                Logging.Log.Failure(e.ToString());
            }
        }
    }
}
