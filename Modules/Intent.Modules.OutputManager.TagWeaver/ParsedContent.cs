using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.OutputManager.TagWeaver
{
    public class ParsedContent
    {
        private StringBuilder _buffer;
        private List<ParsedToken> _tokens;

        public List<ParsedToken> Tokens
        {
            get { return _tokens; }
            set { _tokens = value; }
        }

        public ParsedContent(string content, List<ParsedToken> tokens)
        {
            _buffer = new StringBuilder(content);
            _tokens = tokens;
        }

        public void RemoveToken(ParsedToken token)
        {
            _tokens.Remove(token);
            RemoveContent(token.Index, token.Length);
        }

        public void AppendContent(string newContent, int offset)
        {
            _buffer.Insert(offset, newContent);
            AdjustTokens(offset, newContent.Length);
        }

        public void RemoveContent(int offset, int length)
        {
            _buffer.Remove(offset, length);
            AdjustTokens(offset, -1 * length);
        }

        public string GetContentBetween(ParsedToken from, ParsedToken to)
        {
            int start = from.Index + from.Length;
            return _buffer.ToString(start, (to.Index - start) + 1 );
        }

        public void ReplaceContentBetween(ParsedToken from, ParsedToken to, string newContent)
        {
            int start = from.Index + from.Length;
            int end = (to.Index - start) + 1;
            RemoveContent(start, end);
            AppendContent(newContent, start);
        }

        private void AdjustTokens(int offset, int length)
        {
            foreach(var token in _tokens)
            {
                if (token.Index >= offset)
                {
                    token.Index += length;
                }
            }
        }

        public override string ToString()
        {
            return _buffer.ToString();
        }
    }
}
