using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypescriptFileImport : TypeScriptNode
    {
        public TypescriptFileImport(Node node, TypeScriptFile file) : base(node, file)
        {
        }

        public string Location => Node.OfKind(SyntaxKind.StringLiteral).SingleOrDefault()?.GetText();
        public string[] Types => Node.OfKind(SyntaxKind.ImportSpecifier).Select(x => x.IdentifierStr).ToArray();

        public bool HasType(string typeName)
        {
            return Types.Contains(typeName);
        }

        public override bool IsIgnored()
        {
            return false;
        }

        internal override void UpdateNode()
        {
            Node = FindNodes(File.Ast.RootNode, Path).First(x => x.OfKind(SyntaxKind.StringLiteral).Single().GetText() == Location);
        }
    }
}