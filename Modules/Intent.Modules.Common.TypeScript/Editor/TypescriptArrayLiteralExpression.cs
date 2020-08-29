using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptArrayLiteralExpression : TypeScriptNode
    {
        public TypeScriptArrayLiteralExpression(Node node, TypeScriptFile file) : base(node, file)
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
                        return new TypeScriptObjectLiteralExpression(x, File);
                    case SyntaxKind.ArrayLiteralExpression:
                        return new TypeScriptArrayLiteralExpression(x, File);
                    case SyntaxKind.FirstLiteralToken:
                    case SyntaxKind.StringLiteral:
                        return new TypescriptLiteral(x, File);
                    default:
                        return null;
                }
            }).Select(x => x as T).ToList();
        }

        public void AddValue(string literal)
        {
            if (!Node.Children.Any())
            {
                Change.ChangeNode(Node, $@"[
  {literal}
]");
            }
            else
            {
                Change.InsertAfter(Node.Children.Last(), $@",
  {literal}");
            }
            UpdateChanges();
        }

        public override bool IsIgnored() => false;

    }

    public class TypescriptLiteral : TypeScriptNode
    {
        public TypescriptLiteral(Node node, TypeScriptFile file) : base(node, file)
        {
        }

        public string Value => Node.GetText();

        public override bool IsIgnored() => false;
    }
}