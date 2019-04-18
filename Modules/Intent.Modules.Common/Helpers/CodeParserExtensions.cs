namespace Intent.Modules.Common
{
    public static class CodeParserExtensions
    {
        /// <summary>
        /// Very basic code scope identifier
        /// </summary>
        /// <param name="source"></param>
        /// <param name="identifier"></param>
        /// <param name="startBracket"></param>
        /// <param name="endBracket"></param>
        /// <returns></returns>
        public static string CodeInside(this string source, string identifier, char startBracket, char endBracket)
        {
            var indexes = source.IndexesOfCodeInside(identifier, startBracket, endBracket);
            return source.Slice(indexes.StartIndex, indexes.EndIndex);
        }

        public static string MethodParameters(this string source, string methodName)
        {
            return source.CodeInside(methodName, '(', ')');
        }

        public static string MethodBody(this string source, string methodName)
        {
            return source.CodeInside(methodName, '{', '}');
        }

        public static string ClassBody(this string source, string className)
        {
            return source.CodeInside("class " + className, '{', '}');
        }

        public static string ReplaceCodeInside(this string source, string identifier, char startBracket, char endBracket, string replaceWith)
        {
            var indexes = source.IndexesOfCodeInside(identifier, startBracket, endBracket);
            source = source.Remove(indexes.StartIndex, indexes.EndIndex - indexes.StartIndex);
            return source.Insert(indexes.StartIndex, replaceWith);
        }

        public static SubstringIndexes IndexesOfCodeInside(this string source, string identifier, char startBracket, char endBracket)
        {
            var i = source.IndexOf(identifier) + identifier.Length;
            var startIndex = -1;
            var indented = 0;
            while (true)
            {
                var currentChar = source[i];
                if (currentChar == startBracket)
                {
                    if (indented == 0)
                    {
                        startIndex = i + 1;
                    }

                    indented++;
                }

                if (currentChar == endBracket)
                {
                    indented--;
                    if (indented == 0)
                    {
                        return new SubstringIndexes(startIndex, i);
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// Get the string slice between the two indexes.
        /// Inclusive for start index, exclusive for end index.
        /// </summary>
        public static string Slice(this string source, int start, int end)
        {
            if (end < 0) // Keep this for negative end support
            {
                end = source.Length + end;
            }
            int len = end - start;               // Calculate length
            return source.Substring(start, len); // Return Substring of length
        }

        
    }

    public class SubstringIndexes
    {
        public SubstringIndexes(int startIndex, int endIndex)
        {
            StartIndex = startIndex;
            EndIndex = endIndex;
        }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}