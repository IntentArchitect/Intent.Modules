using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptFileImport : TypeScriptNode
    {
        public TypeScriptFileImport(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
            Location = Node.OfKind(SyntaxKind.StringLiteral).SingleOrDefault()?.GetText();
            Types = Node.OfKind(SyntaxKind.ImportSpecifier).Select(x => x.IdentifierStr).ToList();
        }

        public string Location { get; }
        public IList<string> Types { get; }
        public override string Identifier => GetIdentifier(Types, Location);

        public bool HasType(string typeName)
        {
            return Types.Contains(typeName);
        }

        public void AddType(string typeName)
        {
            Types.Add(typeName);
            Editor.Change.InsertAfter(Node.OfKind(SyntaxKind.ImportSpecifier).Last(), $", {typeName}");
            Editor.UpdateNodes();
        }

        public override string GetIdentifier(Node node)
        {
            var types = node.OfKind(SyntaxKind.ImportSpecifier).Select(x => x.IdentifierStr).ToArray();
            var location = node.OfKind(SyntaxKind.StringLiteral).SingleOrDefault()?.GetText();
            return GetIdentifier(types, location);
        }

        private string GetIdentifier(IList<string> types, string location)
        {
            return $"import {{{string.Join(", ", types.OrderBy(x => x))}}} from {location};";
        }

        public override bool IsIgnored()
        {
            return false;
        }

    }
}