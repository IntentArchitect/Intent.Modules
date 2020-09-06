using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptFileImport : TypeScriptNode
    {
        public TypeScriptFileImport(Node node, TypeScriptFile file) : base(node, file)
        {
            Location = Node.OfKind(SyntaxKind.StringLiteral).SingleOrDefault()?.GetText();
            Types = Node.OfKind(SyntaxKind.ImportSpecifier).Select(x => x.IdentifierStr).ToArray();
        }

        public string Location { get; }
        public string[] Types { get; }

        public bool HasType(string typeName)
        {
            return Types.Contains(typeName);
        }

        public void AddType(string typeName)
        {
            Change.InsertAfter(Node.OfKind(SyntaxKind.ImportSpecifier).Last(), $", {typeName}");
        }

        public override string GetIdentifier(Node node)
        {
            return $"import {{{string.Join(", ", Types.OrderBy(x => x))}}} from {Location};";
        }

        public override bool IsIgnored()
        {
            return false;
        }

    }
}