using System;
using System.Diagnostics;
using Intent.Code.Weaving.Java;
using Intent.Code.Weaving.Java.Editor;
using Intent.Engine;
using Intent.Modules.Common.Java.Templates;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.Java.Weaving
{
    /// <summary>
    /// An <see cref="ITransformOutput"/> which will weave changes between an existing file
    /// and the output of a template respecting annotations in the files.
    /// </summary>
    public class JavaWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        /// <inheritdoc />
        public override string Id => "Intent.Common.Java.OutputWeaver";

        /// <inheritdoc />
        public bool CanTransform(IOutputFile output)
        {
            return output.Template is IJavaMerged;
        }

        /// <inheritdoc />
        public void Transform(IOutputFile output)
        {
            if (!(output.Template is IJavaMerged))
            {
                throw new InvalidOperationException($"Cannot transform outputs where the template does not derive from {nameof(IJavaMerged)}");
            }

            var existingFile = output.PreviousFilePathExists()
                ? JavaFile.Parse(output.GetPreviousFilePathContent())
                : null;

            if (existingFile == null)
            {
                return;
            }

            try
            {
                var merger = new JavaWeavingMerger();

                var sw = Stopwatch.StartNew();
                var newContent = merger.Merge(existingFile, output.Content);
                Logging.Log.Debug($"Merged file: {output.FileMetadata.GetFilePath()} ({sw.ElapsedMilliseconds}ms)");

                output.ChangeContent(newContent);
            }
            catch (Exception e)
            {
                output.ChangeContent(existingFile.GetSource());

                Logging.Log.Failure($"Error while weaving Java file: {output.FileMetadata.GetFilePath()}");
                Logging.Log.Failure(e.ToString());
                Logging.Log.Failure($@"EXISTING CONTENT:
{output.GetExistingFileContent()}

-------------------------------------------------------

{output.Content}");
            }
        }
    }
}
