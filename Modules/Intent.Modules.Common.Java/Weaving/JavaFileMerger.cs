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

            //foreach (var import in _outputFile.Imports())
            //{
            //    if (!_existingFile.ImportExists(import))
            //    {
            //        _existingFile.AddImport(import);
            //    }
            //}

            //if (_existingFile.Ast.RootNode.OfKind(SyntaxKind.Decorator).All(x => !x.First.IdentifierStr.StartsWith("Intent")))
            //{
            //    return _outputFile.GetSource();
            //}

            var existingClasses = _existingFile.Classes;
            var outputClasses = _outputFile.Classes;

            MergeNodes(existingClasses, outputClasses);

            return _existingFile.GetSource();
        }

        private void MergeNodes(IEnumerable<JavaNode> existingNodes, IEnumerable<JavaNode> outputNodes)
        {
            var toAdd = outputNodes.Except(existingNodes).ToList();
            var toUpdate = existingNodes.Where(x => !x.IsIgnored()).Intersect(outputNodes).ToList();
            var toRemove = existingNodes.Where(x => !x.IsIgnored()).Except(outputNodes).ToList();

            foreach (var existingNode in toUpdate)
            {
                var outputNode = outputNodes.Single(x => x.Equals(existingNode));
                if (existingNode.Children.All(x => !x.IsIgnored()))
                {
                    existingNode.ReplaceWith(outputNode.GetText());
                    continue;
                }
                MergeNodes(outputNode.Children, existingNode.Children);
            }

            foreach (var node in toAdd)
            {
                var text = node.GetText();
                if (text.TrimStart().Length == text.Length)
                {
                    text = $"{Environment.NewLine}{Environment.NewLine}{text}";
                }
                _existingFile.InsertAfter(existingNodes.Last(), text);
            }

            foreach (var node in toRemove)
            {
                node.Remove();
            }
        }
    }
}