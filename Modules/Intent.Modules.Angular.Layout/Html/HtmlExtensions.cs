using System;
using System.Collections.Generic;
using System.Text;
using HtmlAgilityPack;

namespace Intent.Modules.Angular.Layout.Html
{
    public static class HtmlExtensions
    {
        public static void AppendChild(this HtmlNode node, string html)
        {
            var htmlDoc = new HtmlDocument()
            {
                OptionOutputOriginalCase = true,
                OptionWriteEmptyNodes = true
            };
            htmlDoc.LoadHtml($@"
    <wrapper>  {html.Trim()}
    </wrapper>");
            node.AppendChildren(htmlDoc.DocumentNode.SelectSingleNode("//wrapper").ChildNodes);
        }
    }
}
