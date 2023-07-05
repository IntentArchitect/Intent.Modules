using System;
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

            if (!output.PreviousFilePathExists())
            {
                return;
            }

            var existingContent = output.GetPreviousFilePathContent();

            TypeScriptFile existingFile;
            try
            {
                existingFile = new TypeScriptFileEditor(existingContent).File;
            }
            catch (Exception e)
            {
                Logging.Log.Failure($"An exception occurred when trying to parse the TypeScript file at [{output.PreviousFilePath}]: " +
                                    $"{Environment.NewLine}" +
                                    $"Template Id: {output.Template.Id}{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- EXISTING CONTENT START ---------------------{Environment.NewLine}" +
                                    $"{existingContent.TrimStart()}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- EXISTING CONTENT FINISH --------------------{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- EXCEPTION DETAILS --------------------------{Environment.NewLine}" +
                                    $"{e}");
                output.ChangeContent(existingContent); // don't change output
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
                Logging.Log.Failure($"An exception occurred when trying to weave the TypeScript file [{output.FileMetadata.GetFullLocationPath()}/{output.FileMetadata.FileNameWithExtension()}]: " +
                                    $"{Environment.NewLine}" +
                                    $"Template Id: {output.Template.Id}{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- EXISTING CONTENT START ---------------------{Environment.NewLine}" +
                                    $"{existingContent.TrimStart()}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- EXISTING CONTENT FINISH --------------------{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- GENERATED CONTENT START --------------------{Environment.NewLine}" +
                                    $"{output.Content}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- GENERATED CONTENT FINISH -------------------{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"{Environment.NewLine}" +
                                    $"--------------------- EXCEPTION DETAILS --------------------------{Environment.NewLine}" +
                                    $"{e}");
                output.ChangeContent(existingContent); // don't change output
            }
        }
    }
}
