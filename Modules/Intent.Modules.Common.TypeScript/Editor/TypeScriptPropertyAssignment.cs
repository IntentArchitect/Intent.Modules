using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptPropertyAssignment : TypeScriptNode
    {
        private readonly TypeScriptNode _parent;

        public TypeScriptPropertyAssignment(Node node, TypeScriptNode parent) : base(node, parent)
        {
            _parent = parent;
        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }

        public override bool CanAdd()
        {
            return base.HasIntentInstructions() ? base.CanAdd() : _parent.CanAdd();
        }

        public override bool CanUpdate()
        {
            return base.HasIntentInstructions() ? base.CanUpdate() : _parent.CanUpdate();
        }

        public override bool CanRemove()
        {
            return base.HasIntentInstructions() ? base.CanRemove() : _parent.CanRemove();
        }

        public override bool HasIntentInstructions()
        {
            return base.HasIntentInstructions() || _parent.HasIntentInstructions();
        }

        public override void Remove()
        {
            if (_parent.Children.IndexOf(this) > 0)
            {
                Editor.Replace(StartIndex - 1, EndIndex, ""); // to get rid of the comma
                return;
            }
            base.Remove();
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
                    // Don't merge literals:
                    case SyntaxKind.StringLiteral:
                    case SyntaxKind.NumericLiteral:
                    case SyntaxKind.RegularExpressionLiteral:
                        this.InsertOrUpdateChildNode(child, index, () => new TypescriptLiteral(child, this, IdentifyBy.Index));
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