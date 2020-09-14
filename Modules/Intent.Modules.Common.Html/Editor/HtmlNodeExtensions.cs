using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Intent.Modules.Common.Html.Editor
{
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
            var parentPath = node.ParentNode?.XPath;
            return parentPath != null && node.XPath.StartsWith(parentPath)
                ? node.XPath.Substring(parentPath.Length + 1)
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

        public static void ReplaceWith(this HtmlNode node, HtmlNode replaceWith)
        {
            var htmlDoc = new HtmlDocument()
            {
                OptionOutputOriginalCase = true,
                OptionWriteEmptyNodes = true
            };
            htmlDoc.LoadHtml(replaceWith.OuterHtml);
            node.ParentNode.ReplaceChild(htmlDoc.DocumentNode, node);
        }
    }
}