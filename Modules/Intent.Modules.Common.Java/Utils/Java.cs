using System.Collections.Generic;
using Intent.Modules.Common.Java.TypeResolvers;

namespace Intent.Modules.Common.Java
{
    /// <summary>
    /// Helper methods for working with Java templates.
    /// </summary>
    public static class Java
    {
        /// <summary>
        /// If the provided <paramref name="typeName"/> is a primitive, its wrapper type name is
        /// returned, otherwise the original <paramref name="typeName"/> is returned.
        /// </summary>
        /// <remarks>
        /// This is typically used when generating type names for use as generic type parameters
        /// as Java only supports use of reference types.
        /// </remarks>
        public static string AsReferenceType(this string typeName)
        {
            return JavaTypeResolver.TryGetWrapperTypeName(typeName, out var wrapperTypeName)
                ? wrapperTypeName
                : typeName;
        }

        public static readonly HashSet<string> ReservedWords = new HashSet<string>
        {
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
            "using static",
            "virtual",
            "void",
            "volatile",
            "while"
        };
    }
}
