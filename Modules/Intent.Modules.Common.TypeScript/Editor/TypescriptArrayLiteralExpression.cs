using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptArrayLiteralExpression : TypeScriptNode
    {
        public TypeScriptArrayLiteralExpression(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
        }

        public List<T> GetValues<T>()
            where T : TypeScriptNode
        {
            return Node.Children.Select<Node, TypeScriptNode> (x =>
            {
                switch (x.Kind)
                {
                    case SyntaxKind.ObjectLiteralExpression:
                        return new TypeScriptObjectLiteralExpression(x, Editor);
                    case SyntaxKind.ArrayLiteralExpression:
                        return new TypeScriptArrayLiteralExpression(x, Editor);
                    case SyntaxKind.FirstLiteralToken:
                    case SyntaxKind.StringLiteral:
                        return new TypescriptLiteral(x, Editor);
                    default:
                        return null;
                }
            }).Select(x => x as T).ToList();
        }

        public void AddValue(string literal)
        {
            if (!Node.Children.Any())
            {
                Editor.Change.ChangeNode(Node, $@"[
  {literal}
]");
            }
            else
            {
                Editor.Change.InsertAfter(Node.Children.Last(), $@",
  {literal}");
            }
            Editor.UpdateNodes();
        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }

        public override bool IsIgnored() => false;
    }

    public class TypescriptLiteral : TypeScriptNode
    {
        public TypescriptLiteral(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
        }

        public string Value => Node.GetText();

        public override string GetIdentifier(Node node)
        {
            return node.GetText();
        }

        public override bool IsIgnored() => false;
    }
}