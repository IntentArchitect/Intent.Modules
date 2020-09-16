
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HtmlAgilityPack;

namespace Intent.Modules.Common.Html.Editor
{
    public class HtmlFileEditor
    {

    }

    public class HtmlFile
    {
        private readonly HtmlDocument _doc;

        public HtmlFile(string source)
        {
            _doc = new HtmlDocument()
            {
                OptionOutputOriginalCase = true,
                OptionWriteEmptyNodes = true
            };
            _doc.LoadHtml(source);
        }

        public HtmlNode Node => _doc.DocumentNode;

        public void MergeWith(HtmlFile mergeFile)
        {
            MergeNodes(Node, mergeFile.Node);
        }

        private static void MergeNodes(HtmlNode node, HtmlNode mergeNode)
        {
            var index = 0;
            foreach (var mergeChild in mergeNode.ChildElements())
            {
                var path = mergeChild.GetRelativeXPath();
                var existing = node.SelectSingleNode(path);
                if (existing == null)
                {
                    if (node.CanAdd())
                    {
                        node.InsertChildWithWhitespace(index, mergeChild);
                        index++;
                    }
                }
                else
                {
                    var existingIndex = node.ChildElements().IndexOf(existing);
                    index = (existingIndex > index) ? existingIndex + 1 : index + 1;
                    if (existing.IsIgnored())
                    {
                        continue;
                    }
                    if (node.CanOverwrite() && !existing.HasIntentAttribute())
                    {
                        existing.ReplaceWith(mergeChild);
                        continue;
                    }
                    MergeNodes(existing, mergeChild);
                }
            }

            if (node.CanRemove())
            {
                var toRemoves = node.ChildElements().Where(x => mergeNode.SelectSingleNode(x.GetRelativeXPath()) == null).ToList();
                node.RemoveWithWhitespace(toRemoves.Where(x => !x.IsIgnored()).ToArray());
            }
        }

        public string GetSource()
        {
            using (var writer = new StringWriter())
            {
                _doc.Save(writer);
                return writer.ToString();
            }
        }
    }
}