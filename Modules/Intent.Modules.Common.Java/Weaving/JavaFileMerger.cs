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

            if (_outputFile.Package != null)
            {
                _existingFile.SetPackage(_outputFile.Package);
            }

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

            //MergeNodes(_existingFile, _outputFile);
            _existingFile.MergeWith(_outputFile);

            return _existingFile.GetSource();
        }

        private void MergeNodes(JavaNode existingNode, JavaNode outputNode)
        {
            MergeAnnotations(existingNode, outputNode);

            var index = 0;
            foreach (var node in outputNode.Children)
            {
                var existing = existingNode.TryGetChild(node.Context);
                if (existing == null)
                {
                    // toAdd:
                    var text = node.GetTextWithComments();
                    if (existingNode.Children.Count == 0)
                    {
                        // EOF has Stop == null
                        _existingFile.InsertBefore(existingNode.Context.Stop ?? existingNode.Context.Start, text);
                    }
                    else if (index == 0)
                    {
                        existingNode.InsertBefore(existingNode.Children[0], node);
                    }
                    else if (existingNode.Children.Count > index)
                    {
                        existingNode.InsertAfter(existingNode.Children[index - 1], node);
                    }
                    else
                    {
                        existingNode.InsertAfter(existingNode.Children.Last(), node);
                    }

                    index++;
                }
                else
                {
                    // toUpdate:
                    var existingIndex = existingNode.Children.IndexOf(existing);
                    index = (existingIndex > index) ? existingIndex + 1 : index + 1;
                    if (existing.IsIgnored())
                    {
                        continue;
                    }

                    if (existing.Children.All(x => !x.IsIgnored()) && !existing.IsMerged())
                    {
                        existing.ReplaceWith(node.GetTextWithComments()); // Overwrite
                        continue;
                    }
                    MergeNodes(existing, node);
                }
            }

            if (!existingNode.IsMerged())
            {
                var toRemove = existingNode.Children.Where(x => !x.IsIgnored()).Except(outputNode.Children).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }

        private void MergeAnnotations(JavaNode existingNode, JavaNode outputNode)
        {
            var index = 0;
            foreach (var node in outputNode.Annotations)
            {
                var existing = existingNode.TryGetAnnotation((Java9Parser.AnnotationContext) node.Context);
                if (existing == null)
                {
                    // toAdd:
                    //var text = node.GetTextWithComments();
                    if (existingNode.Annotations.Count == 0)
                    {
                        _existingFile.InsertBefore(existingNode, node.GetTextWithComments());
                    }
                    else if (index == 0)
                    {
                        existingNode.InsertBefore(existingNode.Annotations[0], node);
                    }
                    else if (existingNode.Annotations.Count > index)
                    {
                        existingNode.InsertAfter(existingNode.Annotations[index - 1], node);
                    }
                    else
                    {
                        existingNode.InsertAfter(existingNode.Annotations.Last(), node);
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

                    if (existing.Annotations.All(x => !x.IsIgnored()) && !existing.IsMerged())
                    {
                        existing.ReplaceWith(node.GetTextWithComments()); // Overwrite
                        continue;
                    }
                    //MergeDecorators(existing, node); // maybe one day
                }
            }

            if (!existingNode.IsMerged())
            {
                var toRemove = existingNode.Annotations.Where(x => !x.IsIgnored()).Except(outputNode.Annotations).ToList();
                foreach (var node in toRemove)
                {
                    node.Remove();
                }
            }
        }
    }
}