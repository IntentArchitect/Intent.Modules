using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptClassDecorator : TypescriptNode
    {
        public TypescriptClassDecorator(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public string Name => Node.First.IdentifierStr;

        public IEnumerable<TypescriptObjectLiteralExpression> Parameters()
        {
            return Node.OfKind(SyntaxKind.ObjectLiteralExpression).Select(x => new TypescriptObjectLiteralExpression(x, File));
        }
    }
}