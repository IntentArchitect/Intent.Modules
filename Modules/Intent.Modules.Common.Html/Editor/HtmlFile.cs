
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;

namespace Intent.Modules.Common.Html.Editor
{
    public class HtmlFileEditor
    {

    }

    public class HtmlFile
    {
        private HtmlDocument _doc;

        public HtmlFile(string source)
        {
            _doc = new HtmlDocument()
            {
                OptionOutputOriginalCase = true,
                OptionWriteEmptyNodes = true,
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
            if (mergeNode.NodeType != HtmlNodeType.Element)
            {
                foreach (var mergeChild in mergeNode.ChildNodes.Where(x => x.NodeType == HtmlNodeType.Element).ToList())
                {
                    MergeNodes(node, mergeChild);
                }

                return;
            }
            var path = mergeNode.GetRelativeXPath();
            var exiting = node.SelectSingleNode(path);
            if (exiting == null)
            {
                if (node.IsMerged())
                {
                    node.AppendChildWithWhitespace(mergeNode);
                }
            }
            else
            {
                if (exiting.IsIgnored())
                {
                    return;
                }
                if (exiting.IsManaged())
                {
                    exiting.ReplaceWith(mergeNode);
                    return;
                }
                foreach (var mergeChild in mergeNode.ChildNodes.Where(x => x.NodeType == HtmlNodeType.Element).ToList())
                {
                    MergeNodes(exiting, mergeChild);
                }
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

    public class IntentHtmlNode : IEquatable<IntentHtmlNode>
    {
        private readonly HtmlNode _node;

        public IntentHtmlNode(HtmlNode node)
        {
            _node = node;
            Identifier = _node.GetAttributeValue("intent-ignore", null) ??
                         _node.GetAttributeValue("intent-merge", null) ??
                         _node.GetAttributeValue("intent-manage", null) ??
                         $"{_node.XPath}";
        }

        public string Identifier { get; }
        public IList<IntentHtmlNode> Children = new List<IntentHtmlNode>();

        public void MergeWith(IntentHtmlNode file)
        {

        }

        public bool IsIgnored()
        {
            return _node.GetAttributeValue("intent-ignore", null) != null;
        }

        public bool IsMerged()
        {
            return _node.GetAttributeValue("intent-merge", null) != null;
        }

        public bool IsManaged()
        {
            return _node.GetAttributeValue("intent-manage", null) != null;
        }

        public bool Equals(IntentHtmlNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Identifier == other.Identifier;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IntentHtmlNode)obj);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}