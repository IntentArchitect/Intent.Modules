using System;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypescriptLiteral : TypeScriptNode
    {
        private readonly IdentifyBy _identifyBy;

        public TypescriptLiteral(Node node, TypeScriptNode parent, IdentifyBy identifyBy) : base(node, parent)
        {
            _identifyBy = identifyBy;
        }

        public string Value => Node.GetText();

        public override string GetIdentifier(Node node)
        {
            switch (_identifyBy)
            {
                case IdentifyBy.Name:
                    return node.GetText();
                case IdentifyBy.Index:
                    return node.Parent.Children.IndexOf(node).ToString();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool IsIgnored() => false;


        public override bool CanAdd()
        {
            return base.HasIntentInstructions() ? base.CanAdd() : Parent.CanAdd();
        }

        public override bool CanUpdate()
        {
            return base.HasIntentInstructions() ? base.CanUpdate() : Parent.CanUpdate();
        }

        public override bool CanRemove()
        {
            return base.HasIntentInstructions() ? base.CanRemove() : Parent.CanRemove();
        }

        public override bool HasIntentInstructions()
        {
            return base.HasIntentInstructions() || Parent.HasIntentInstructions();
        }

    }

    public enum IdentifyBy
    {
        Name,
        Index
    }
}