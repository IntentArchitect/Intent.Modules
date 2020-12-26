using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Intent.Code.Weaving.TypeScript;
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

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is ITypeScriptMerged typeScriptMerged))
            {
                return;
            }
            try
            {
                var existingFile = typeScriptMerged.GetExistingFile();

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
