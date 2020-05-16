using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptClassDecorator : TypeScriptNode
    {
        public TypeScriptClassDecorator(Node node, TypeScriptFile file) : base(node, file)
        {
        }

        public string Name => Node.First.IdentifierStr;

        public IEnumerable<TypeScriptObjectLiteralExpression> Parameters()
        {
            return Node.OfKind(SyntaxKind.ObjectLiteralExpression).Select(x => new TypeScriptObjectLiteralExpression(x, File));
        }
    }
}