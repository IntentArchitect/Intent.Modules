using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Intent.SoftwareFactory.Plugins.TagWeaving
{
    public class ContentParser
    {
        private List<TokenParser> _registeredTokens;

        public ContentParser(List<TokenParser> tokenTypes)
        {
            _registeredTokens = tokenTypes;
        }

        public ParsedContent Parse(string content)
        {
            
            var regEx = RegexCache.Get(string.Join("|", _registeredTokens.Select(t => "(?<" + t.Name + ">" + t.Pattern + ")").ToArray()));
            var matches = regEx.Matches(content);

            List<ParsedToken> tokens = new List<ParsedToken>();

            foreach (Match match in matches)
            {
                foreach (var t in _registeredTokens)
                {
                    if (match.Groups[t.Name] != null)
                    {
                        var m = match.Groups[t.Name];
                        if (m.Success)
                        {
                            ParsedToken p = t.Parse(m.Index, m.Length, m.Value );
                            tokens.Add(p);
                        }
                    }
                }
            }
            return new ParsedContent(content, tokens);
        }
    }
}
