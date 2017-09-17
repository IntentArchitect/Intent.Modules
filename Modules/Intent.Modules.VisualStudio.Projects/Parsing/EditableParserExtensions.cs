using System;

namespace Intent.Modules.VisualStudio.Projects.Parsing
{
    public static class EditableParserExtensions
    {
        public static bool PeekToken(this EditableParser parser,  string match)
        {
            var token = parser.PeekToken();
            return token == match;
        }

        public static string GetToken(this EditableParser parser, string expected)
        {
            var token = parser.GetToken();
            if (token != expected)
            {
                throw new Exception($"Invalid Token : {token} expected : {expected}");
            }
            return token;
        }

        public static bool Seek(this EditableParser parser, string token)
        {
            return parser.Seek(parseToken => parseToken == token);
        }

        public static bool SeekStartsWith(this EditableParser parser, string partial)
        {
            return parser.Seek(parseToken => parseToken.StartsWith(partial));
        }

        public static void Insert(this EditableParser parser, string content)
        {
            parser.Insert(parser.Position, content, true);
        }
    }
}
