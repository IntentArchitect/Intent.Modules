using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypescriptLiteral : TypeScriptNode
    {
        public TypescriptLiteral(Node node, TypeScriptNode parent) : base(node, parent.Editor)
        {
        }

        public string Value => Node.GetText();

        public override string GetIdentifier(Node node)
        {
            return node.Parent.Children.IndexOf(node).ToString();
        }

        public override bool IsIgnored() => false;
    }
}