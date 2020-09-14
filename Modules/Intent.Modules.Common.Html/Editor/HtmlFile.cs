
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
                    exiting.ParentNode.ReplaceChild(HtmlNode.CreateNode(mergeNode.OuterHtml), exiting);
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

    public static class HtmlNodeExtensions
    {
        public static string GetRelativeXPath(this HtmlNode node)
        {
            var identifier = node.GetAttributeValue("intent-ignore", null) ??
                             node.GetAttributeValue("intent-merge", null) ??
                             node.GetAttributeValue("intent-manage", null);
            if (identifier != null)
            {
                return $"//*[@intent-manage='{identifier}' or @intent-merge='{identifier}' or @intent-manage='{identifier}']";
            }
            var parentPath = node.ParentNode?.GetRelativeXPath();
            return parentPath != null && node.XPath.StartsWith(parentPath)
                ? node.XPath.Substring(parentPath.Length)
                : node.XPath;
        }

        public static bool IsIgnored(this HtmlNode node)
        {
            return node.GetAttributeValue("intent-ignore", null) != null;
        }

        public static bool IsMerged(this HtmlNode node)
        {
            return node.GetAttributeValue("intent-merge", null) != null;
        }

        public static bool IsManaged(this HtmlNode node)
        {
            return node.GetAttributeValue("intent-manage", null) != null;
        }

        public static bool HasIntentAttribute(this HtmlNode node)
        {
            return node.IsIgnored() || node.IsMerged() || node.IsManaged();
        }

        public static void AppendChildWithWhitespace(this HtmlNode node, HtmlNode newNode)
        {
            var leadingWs = Regex.Match(newNode.ParentNode.InnerHtml, "^\\s*").Value;
            var trailingWs = Regex.Match(newNode.ParentNode.InnerHtml, "\\s*$").Value;
            node.AppendChild($"{leadingWs}{newNode.OuterHtml}{trailingWs}");
        }

        public static void AppendChild(this HtmlNode node, string html)
        {
            var htmlDoc = new HtmlDocument()
            {
                OptionOutputOriginalCase = true,
                OptionWriteEmptyNodes = true
            };
            htmlDoc.LoadHtml(string.IsNullOrEmpty(node.InnerHtml)
                ? $@"<wrapper>{html}</wrapper>"
                : $@"<wrapper>  {html.TrimStart()}</wrapper>");
            node.AppendChildren(htmlDoc.DocumentNode.SelectSingleNode("//wrapper").ChildNodes);
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