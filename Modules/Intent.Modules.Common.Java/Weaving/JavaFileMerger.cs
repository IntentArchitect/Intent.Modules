using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Modules.Common.Java.Editor;

namespace Intent.Modules.Common.Java.Weaving
{
    public class JavaFileMerger
    {
        private readonly JavaFile _existingFile;
        private readonly JavaFile _outputFile;

        public JavaFileMerger(JavaFile existingFile, JavaFile outputFile)
        {
            _existingFile = existingFile;
            _outputFile = outputFile;
        }

        public string GetMergedFile()
        {
            //if (_existingFile.GetSource().TrimStart().StartsWith("//@IntentIgnoreFile()"))
            //{
            //    return _existingFile.GetSource();
            //}

            foreach (var import in _outputFile.Imports)
            {
                if (!_existingFile.ImportExists(import))
                {
                    _existingFile.AddImport(import);
                }
            }

            //if (_existingFile.Ast.RootNode.OfKind(SyntaxKind.Decorator).All(x => !x.First.IdentifierStr.StartsWith("Intent")))
            //{
            //    return _outputFile.GetSource();
            //}

            MergeNodes(_existingFile, _outputFile);

            return _existingFile.GetSource();
        }

        private void MergeNodes(JavaNode existingNode, JavaNode outputNode)
        {
            var existingNodes = existingNode.Children;
            var outputNodes = outputNode.Children;
            var toAdd = outputNodes.Except(existingNodes).ToList();
            var toUpdate = existingNodes.Where(x => !x.IsIgnored()).Intersect(outputNodes).ToList();
            var toRemove = existingNodes.Where(x => !x.IsIgnored()).Except(outputNodes).ToList();

            foreach (var update in toUpdate)
            {
                var inOutput = outputNodes.Single(x => x.Equals(update));
                if (update.Children.All(x => !x.IsIgnored()))
                {
                    update.ReplaceWith(inOutput.GetText());
                    continue;
                }
                MergeNodes(update, inOutput);
            }

            foreach (var node in toAdd)
            {
                var text = node.GetText();
                _existingFile.InsertAfter(existingNode.Children.Last(), text);
            }

            foreach (var node in toRemove)
            {
                node.Remove();
            }
        }
    }
}