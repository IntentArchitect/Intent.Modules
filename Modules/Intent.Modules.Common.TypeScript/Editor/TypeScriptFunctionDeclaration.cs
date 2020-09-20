using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptFunctionDeclaration : TypeScriptNode
    {
        public TypeScriptFunctionDeclaration(Node node, TypeScriptNode parent) : base(node, parent)
        {

        }

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }
    }
}