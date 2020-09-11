using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptDecorator : TypeScriptNode
    {
        private readonly TypeScriptNode _parent;

        public TypeScriptDecorator(Node node, TypeScriptNode parent) : base(node, parent.Editor)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                throw new Exception("Decorator name could not be determined");
            }

            _parent = parent;
        }

        public string Name => Node.First.IdentifierStr;

        public override string GetIdentifier(Node node)
        {
            return node.First.IdentifierStr;
        }

        public IEnumerable<TypeScriptObjectLiteralExpression> Parameters()
        {
            return Node.OfKind(SyntaxKind.ObjectLiteralExpression).Select(x => new TypeScriptObjectLiteralExpression(x, this));
        }

        public override bool IsIgnored()
        {
            return _parent.IsIgnored();
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
            foreach (var child in node.Children.SelectMany(x => x.Children))
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
                    //case SyntaxKind.StringLiteral:
                    //    this.InsertOrUpdateChildNode(child, index, () => new TypescriptLiteral(child, this, IdentifyBy.Index));
                    //    index++;
                    //    continue;
                    //case SyntaxKind.NumericLiteral:
                    //    this.InsertOrUpdateChildNode(child, index, () => new TypescriptLiteral(child, this, IdentifyBy.Index));
                    //    index++;
                    //    continue;
                    //case SyntaxKind.RegularExpressionLiteral:
                    //    this.InsertOrUpdateChildNode(child, index, () => new TypescriptLiteral(child, this, IdentifyBy.Index));
                    //    index++;
                    //    continue;
                }
            }
        }

        public override void InsertBefore(TypeScriptNode existing, TypeScriptNode node)
        {
            Editor.InsertBefore(existing, $"{node.GetTextWithComments()},");
        }

        public override void InsertAfter(TypeScriptNode existing, TypeScriptNode node)
        {
            Editor.InsertAfter(existing, $",{node.GetTextWithComments()}");
        }
    }
}