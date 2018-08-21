using System.Collections.Generic;
using System.Linq;
using Intent.MetaModel.Common;
using Intent.SoftwareFactory.MetaData;

namespace Intent.Modules.Application.Contracts
{
    public static class CSharpHelpers
    {
        // From https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/
        private static readonly string[] CsharpKeywords = {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "using",
            "static",
            "virtual",
            "void",
            "volatile",
            "while"
        };

        public static string PrefixIdentifierIfKeyword(this string identifier)
        {
            return CsharpKeywords.Contains(identifier)
                ? $"@{identifier}"
                : identifier;
        }

        internal static IEnumerable<string> GetXmlDocLines(this IHasStereotypes hasStereotypes)
        {
            var text = hasStereotypes.GetStereotypeProperty<string>("XmlDoc", "Content");

            return string.IsNullOrWhiteSpace(text)
                ? new string[0]
                : text
                    .Replace("\r\n", "\r")
                    .Replace("\n", "\r")
                    .Split('\r');
        }
    }
}
