using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptObjectLiteralExpression : TypeScriptNode
    {
        private readonly TypeScriptNode _parent;
        private readonly string _indexInParent;

        public TypeScriptObjectLiteralExpression(Node node, TypeScriptNode parent) : base(node, parent.Editor)
        {
            _parent = parent;
        }

        public override string GetIdentifier(Node node)
        {
            return node.Parent.Children.IndexOf(node).ToString();
        }

        public override bool IsMerged()
        {
            return _parent.IsMerged();
        }

        public override void MergeWith(TypeScriptNode node)
        {
            base.MergeWith(node);
        }

        public override void InsertBefore(TypeScriptNode existing, TypeScriptNode node)
        {
            Editor.InsertBefore(existing, $"{node.GetTextWithComments()},");
        }

        public override void InsertAfter(TypeScriptNode existing, TypeScriptNode node)
        {
            Editor.InsertAfter(existing, $",{node.GetTextWithComments()}");
        }

        public override void UpdateNode(Node node)
        {
            base.UpdateNode(node);
            var index = 0;
            foreach (var child in node.Children)
            {
                switch (child.Kind)
                {
                    case SyntaxKind.PropertyAssignment:
                        this.InsertOrUpdateChildNode(child, index, () => new TypeScriptPropertyAssignment(child, this));
                        index++;
                        continue;
                }
            }
        }

        public bool PropertyAssignmentExists(string propertyName, string valueLiteral = null)
        {
            var properties = Node.Children.Where(x => x.Kind == SyntaxKind.PropertyAssignment);

            var property = properties.FirstOrDefault(x => x.IdentifierStr == propertyName);
            if (property == null)
            {
                return false;
            }
            return valueLiteral == null || property.Children[1].IdentifierStr == valueLiteral || property.Children[1].GetText() == valueLiteral;
        }

        public void InsertPropertyAssignment(string propertyAssignment, Node afterNode)
        {
            if (afterNode != null)
            {
                if (afterNode.Kind == SyntaxKind.PropertyAssignment)
                {
                    propertyAssignment = $", {propertyAssignment}";
                }
                Editor.Change.InsertAfter(afterNode, propertyAssignment);
            }
            else
            {
                if (Node.Children.OfKind(SyntaxKind.PropertyAssignment).Any())
                {
                    InsertPropertyAssignment(propertyAssignment, Node.OfKind(SyntaxKind.PropertyAssignment).LastOrDefault());
                    return;
                }
                Editor.Change.InsertBefore(Node.Children.Last(), propertyAssignment);
            }
            Editor.UpdateNodes();
        }

        public void AddPropertyAssignment(string propertyAssignment)
        {
            InsertPropertyAssignment(propertyAssignment, Node.OfKind(SyntaxKind.PropertyAssignment).LastOrDefault());
        }
    }
}