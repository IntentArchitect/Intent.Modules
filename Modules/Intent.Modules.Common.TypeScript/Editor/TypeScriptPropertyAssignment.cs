using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptPropertyAssignment : TypeScriptNode
    {
        private readonly TypeScriptNode _parent;

        public TypeScriptPropertyAssignment(Node node, TypeScriptNode parent) : base(node, parent.Editor)
        {
            _parent = parent;
        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }
        public override bool IsMerged()
        {
            return _parent.IsMerged();
        }

        public override void MergeWith(TypeScriptNode node)
        {
            base.MergeWith(node);
        }

        public override void UpdateNode(Node node)
        {
            base.UpdateNode(node);
            var index = 0;
            foreach (var child in node.Children)
            {
                switch (child.Kind)
                {
                    case SyntaxKind.ObjectLiteralExpression:
                        this.InsertOrUpdateChildNode(child, index, () => new TypeScriptObjectLiteralExpression(child, this));
                        index++;
                        continue;
                    case SyntaxKind.ArrayLiteralExpression:
                        this.InsertOrUpdateChildNode(child, index, () => new TypeScriptArrayLiteralExpression(child, this));
                        index++;
                        continue;
                    case SyntaxKind.StringLiteral:
                        this.InsertOrUpdateChildNode(child, index, () => new TypescriptLiteral(child, this));
                        index++;
                        continue;
                    case SyntaxKind.NumericLiteral:
                        this.InsertOrUpdateChildNode(child, index, () => new TypescriptLiteral(child, this));
                        index++;
                        continue;
                    case SyntaxKind.RegularExpressionLiteral:
                        this.InsertOrUpdateChildNode(child, index, () => new TypescriptLiteral(child, this));
                        index++;
                        continue;
                    // OTHERS? Not sure...
                    //case SyntaxKind.TypeLiteral:
                    //    this.InsertOrUpdateNode(node, index, () => new TypescriptLiteral(node, this));
                    //    index++;
                    //    continue;
                }
            }
        }
    }
}