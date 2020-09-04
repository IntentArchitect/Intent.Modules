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

            var index = 0;
            foreach (var node in outputNode.Children)
            {
                var existing = existingNode.TryGetChild(node.Context);
                if (existing == null)
                {
                    // toAdd:
                    var text = node.GetText();
                    if (existingNode.Children.Count == 0)
                    {
                        _existingFile.InsertBefore(existingNode.Context.Stop, text);
                    }
                    else if (index == 0)
                    {
                        existingNode.InsertBefore(existingNode.Children[0], node);
                        //_existingFile.InsertBefore(existingNode.Children[0], text);
                    }
                    else if (existingNode.Children.Count > index)
                    {
                        existingNode.InsertAfter(existingNode.Children[index - 1], node); 
                        //_existingFile.InsertAfter(existingNode.Children[index - 1], text);
                    }
                    else
                    {
                        existingNode.InsertAfter(existingNode.Children.Last(), node);
                        //_existingFile.InsertAfter(existingNode.Children.Last(), text);
                    }

                    index++;
                }
                else
                {
                    // toUpdate:
                    index++;
                    if (existing.IsIgnored())
                    {
                        continue;
                    }

                    if (existing.Children.All(x => !x.IsIgnored()))
                    {
                        existing.ReplaceWith(node.GetText());
                        continue;
                    }
                    MergeNodes(existing, node);
                }
            }

            var toRemove = existingNodes.Where(x => !x.IsIgnored()).Except(outputNode.Children).ToList();
            foreach (var node in toRemove)
            {
                node.Remove();
            }
        }
    }
}