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
            Types = GetTypes(Node);
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
            var types = GetTypes(node);
            var location = node.OfKind(SyntaxKind.StringLiteral).SingleOrDefault()?.GetText();
            return GetIdentifier(types, location);
        }

        private string GetIdentifier(IList<string> types, string location)
        {
            //return location;
            return $"import {{{string.Join(", ", types.OrderBy(x => x))}}} from {location};";
        }

        public override void Remove()
        {
            base.Remove();
            this.Editor.File.Children.Remove(this);
        }

        public override bool IsIgnored()
        {
            return false;
        }

        public IList<string> GetTypes(Node node)
        {
            return node.OfKind(SyntaxKind.ImportSpecifier).Select(x => x.IdentifierStr).ToList();
        }
    }
}