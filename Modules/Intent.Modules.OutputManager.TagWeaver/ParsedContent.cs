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
            return _buffer.ToString(start, (to.Index - start));
        }

        public string GetContentBetween(ParsedToken from, char beginChar, char endChar)
        {
            int start = from.Index + from.Length;
            int end = FindScopeEnd(start, beginChar, endChar);
            return _buffer.ToString(start, end - start);
        }

        private int FindScopeEnd(int start, char beginChar, char endChar)
        {
            var depth = 0;
            var inTheGame = false;
            var text = _buffer.ToString();
            for (int i = start; i < text.Length; i++)
            {
                if (text[i] == beginChar)
                {
                    depth++;
                    inTheGame = true;
                    continue;
                }
                if (text[i] == endChar)
                {
                    depth--;
                }

                if (inTheGame && depth == 0)
                {
                    return i;
                }
            }

            return -1;
        }

        public void ReplaceContentBetween(ParsedToken from, ParsedToken to, string newContent)
        {
            int start = from.Index + from.Length;
            int end = (to.Index - start);
            RemoveContent(start, end);
            AppendContent(newContent, start);
        }

        public void ReplaceContentBetween(ParsedToken from, char beginChar, char endChar, string newContent)
        {
            int start = from.Index + from.Length;
            int end = FindScopeEnd(start, beginChar, endChar) - start;
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
