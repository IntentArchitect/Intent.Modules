using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Intent.Engine;
using Intent.Modules.Common.Plugins;
using Intent.Modules.Common.Templates;
using Intent.Modules.Common.TypeScript.Editor;
using Intent.Modules.Common.TypeScript.Templates;
using Intent.Plugins.FactoryExtensions;
using Intent.Utils;

namespace Intent.Modules.Common.TypeScript.Weaving
{
    public class TypescriptWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.Common.TypeScript.OutputWeaver";

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is ITypeScriptMerged typeScriptMerged))
            {
                return;
            }

            var existingFile = typeScriptMerged.GetExistingFile();

            if (existingFile == null)
            {
                return;
            }

            try
            {
                var merger = new TypeScriptWeavingMerger();

                var newContent = merger.Merge(existingFile, output.Content);

                output.ChangeContent(newContent);
            }
            catch (Exception e)
            {
                Logging.Log.Warning($"Error while weaving TypeScript file: {output.FileMetadata.GetRelativeFilePath()}");
                Logging.Log.Warning(e.ToString());
            }
        }
    }

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
