using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptArrayLiteralExpression : TypescriptNode
    {
        public TypescriptArrayLiteralExpression(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public List<T> GetValues<T>()
            where T : TypescriptNode
        {
            return Node.Children.Select<Node, TypescriptNode> (x =>
            {
                switch (x.Kind)
                {
                    case SyntaxKind.ObjectLiteralExpression:
                        return new TypescriptObjectLiteralExpression(x, File);
                    case SyntaxKind.ArrayLiteralExpression:
                        return new TypescriptArrayLiteralExpression(x, File);
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
        }
    }

    public class TypescriptLiteral : TypescriptNode
    {
        public TypescriptLiteral(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public string Value => Node.GetText();
    }
}