using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptMethod : TypeScriptNode
    {
        public TypeScriptMethod(Node node, TypeScriptFile file) : base(node, file)
        {

        }

        public IList<TypeScriptDecorator> Decorators()
        {
            return Node.OfKind(SyntaxKind.Decorator).Select(x => new TypeScriptDecorator(x, File)).ToList();
        }

        public TypeScriptDecorator GetDecorator(string name)
        {
            return Decorators().SingleOrDefault(x => x.Name == name);
        }

        public string GetTextWithComments()
        {
            return Node.GetTextWithComments();
        }

        public override bool IsIgnored()
        {
            return GetDecorator("IntentIgnore") != null;
        }
    }
}