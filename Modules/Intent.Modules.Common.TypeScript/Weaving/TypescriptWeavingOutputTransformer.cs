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

namespace Intent.Modules.Common.TypeScript.Weaving
{
    public class TypescriptWeavingOutputTransformer : FactoryExtensionBase, ITransformOutput
    {
        public override string Id => "Intent.OutputManager.RoslynWeaver";

        public void Transform(IOutputFile output)
        {
            if (!(output.Template is ITypeScriptMerged typeScriptMerged))
            {
                return;
            }

            var existingFileLocation = output.FileMetadata.GetFullLocationPathWithFileName();
            var existingContent = File.Exists(existingFileLocation)
                ? File.ReadAllText(existingFileLocation)
                : null;

            if (existingContent == null)
            {
                return;
            }

            try
            {
                var merger = new TypeScriptWeavingMerger();

                var newContent = merger.Merge(existingContent, output.Content);

                output.ChangeContent(newContent);
            }
            catch (Exception e)
            {
                throw;
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
            var merger = new TypeScriptFileMerger(new TypeScriptFile(existingContent), new TypeScriptFile(outputContent));
            return merger.GetMergedFile();
        }
    }
}
