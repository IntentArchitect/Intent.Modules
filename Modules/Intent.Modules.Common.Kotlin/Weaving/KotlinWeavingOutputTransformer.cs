using System;
using System.Diagnostics;
using Intent.Code.Weaving.Kotlin;
using Intent.Code.Weaving.Kotlin.Editor;
using Intent.Engine;
using Intent.Modules.Common.Kotlin.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.Kotlin.Weaving
{
    /// <summary>
    /// An <see cref="ITransformOutput"/> which will weave changes between an existing file
    /// and the output of a template respecting annotations in the files.
    /// </summary>
    public class KotlinWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        /// <inheritdoc />
        public override string Id => "Intent.Common.Kotlin.OutputWeaver";

        /// <inheritdoc />
        public bool CanTransform(IOutputFile output)
        {
            return output.Template is IKotlinMerged;
        }

        /// <inheritdoc />
        public void Transform(IOutputFile output)
        {
            if (!(output.Template is IKotlinMerged))
            {
                throw new InvalidOperationException($"Cannot transform outputs where the template does not derive from {nameof(IKotlinMerged)}");
            }

            var existingFile = output.TargetFileExists()
                ? KotlinFile.Parse(output.GetExistingFileContent())
                : null;

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
