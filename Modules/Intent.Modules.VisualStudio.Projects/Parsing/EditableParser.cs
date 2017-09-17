using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Intent.Modules.VisualStudio.Projects.Parsing
{
    public class EditableParser
    {
        private StringBuilder _content;
        private int _pos = 0;
        private List<Bookmark> _bookmarks;
        private const string WHITESPACE = " \r\n\t";

        public EditableParser(StringBuilder content)
        {
            _bookmarks = new List<Bookmark>();
            _content = content;
        }

        private bool IsWhiteSpace()
        {
            return WHITESPACE.Contains(_content[_pos]);
        }

        private bool BOF()
        {
            return _pos == 0;
        }

        private bool EOF()
        {
            return _pos == _content.Length;
        }

        public string PeekToken()
        {
            int start = _pos;
            var result = GetToken();
            _pos = start;
            return result;
        }

        public void Previous()
        {
            GetPreviousToken();
        }

        public int Position
        {
            get
            {
                return _pos;
            }
        }

        public Bookmark CreateBookmark()
        {
            var result = new Bookmark("", _pos);
            _bookmarks.Add(result);
            return result;
        }

        public void ChangePos(int pos)
        {
            _pos = pos;
        }

        public string Content()
        {
            return _content.ToString();
        }

        public string GetValue(int pos, int length)
        {
            return _content.ToString(pos, length);
        }

        public IEnumerable<Bookmark> FindAndBookmark(params string[] tokens)
        {
            var result = new List<Bookmark>();
            while (true)
            {
                int start = _pos;
                var token = GetToken();
                if (token == null)
                {
                    break;
                }
                if (tokens.Contains(token))
                {
                    result.Add(new Bookmark(token, start));
                }
            }
            return result;
        }

        public void Insert(int position, string content, bool movePositionToEndOfInsert)
        {
            foreach (var bookmark in _bookmarks)
            {
                if (bookmark.Position >= position)
                {
                    bookmark.Position += content.Length;
                }
            }
            _content.Insert(position, content);
            if (movePositionToEndOfInsert)
            {
                _pos = position + content.Length;
            }
            else
            {
                if (_pos >= position)
                {
                    _pos += content.Length;
                }
            }
        }

        public void Consume(string expected)
        {
            if (_pos + expected.Length > _content.Length)
            {
                throw new Exception($"Consume : going out of bounds ");
            }
            foreach (var c in expected)
            {
                if (_content[_pos] != c)
                {
                    throw new Exception($"Consume : Expected {c} found {_content[_pos]}");
                }
                _pos++;
            }
        }

        public void Next()
        {
            GetToken();
        }

        public bool Seek(Func<string, bool> matchPredicate)
        {
            int startOfSeek = _pos;
            while (true)
            {
                int start = _pos;
                var result = GetToken();
                if (result == null)
                {
                    _pos = startOfSeek;
                    return false;
                }
                if (matchPredicate(result))
                {
                    _pos = start;
                    return true;
                }
            }
        }

        public string GetPreviousToken()
        {
            while (!BOF() && IsWhiteSpace()) _pos--;
            if (BOF())
            {
                return null;
            }
            int tokenEnd = _pos;
            while (!BOF() && !IsWhiteSpace())
            {
                _pos--;
            }
            return _content.ToString(_pos, tokenEnd - _pos);
        }

        public string GetToken()
        {
            while (!EOF() && IsWhiteSpace()) _pos++;
            if (EOF())
            {
                return null;
            }
            int tokenStart = _pos;
            while (!EOF() && !IsWhiteSpace())
            {
                _pos++;
            }
            return _content.ToString(tokenStart, _pos - tokenStart);
        }

    }
}
