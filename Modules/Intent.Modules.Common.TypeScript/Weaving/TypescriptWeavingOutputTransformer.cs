using System;
using System.IO;
using Intent.Code.Weaving.TypeScript;
using Intent.Code.Weaving.TypeScript.Editor;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.TypeScript.Weaving
{
    public class TypescriptWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.Common.TypeScript.OutputWeaver";

        public bool CanTransform(IOutputFile output)
        {
            return output.Template is ITypeScriptMerged;
        }

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is ITypeScriptMerged))
            {
                throw new InvalidOperationException($"Cannot transform outputs where the template does not derive from {nameof(ITypeScriptMerged)}");
            }

            try
            {
                var existingFile = output.PreviousFilePathExists()
                    ? new TypeScriptFileEditor(output.GetPreviousFilePathContent()).File
                    : null;

                if (existingFile == null)
                {
                    return;
                }

                var merger = new TypeScriptWeavingMerger();

                var newContent = merger.Merge(existingFile, output.Content);

                output.ChangeContent(newContent);
            }
            catch (Exception e)
            {
                var metadata = output.FileMetadata;
                var fullFileName = Path.Combine(metadata.GetFullLocationPath(), metadata.FileNameWithExtension());
                output.ChangeContent(File.ReadAllText(fullFileName)); // don't change output

                Logging.Log.Failure($"Error while weaving TypeScript file: {output.FileMetadata.GetRelativeFilePath()}");
                Logging.Log.Failure(e.ToString());
            }
        }
    }
}
