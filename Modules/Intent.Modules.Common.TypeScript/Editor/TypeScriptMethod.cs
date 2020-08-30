using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptMethod : TypeScriptNode, IEquatable<TypeScriptMethod>
    {
        public TypeScriptMethod(Node node, TypeScriptFile file) : base(node, file)
        {

        }

        public string Name => Node.IdentifierStr;

        public IList<TypeScriptDecorator> Decorators()
        {
            return Node.OfKind(SyntaxKind.Decorator).Select(x => new TypeScriptDecorator(x, File)).ToList();
        }

        public TypeScriptDecorator GetDecorator(string name)
        {
            return Decorators().SingleOrDefault(x => x.Name == name);
        }

        public override bool IsIgnored()
        {
            return GetDecorator("IntentIgnore") != null;
        }

        public bool Equals(TypeScriptMethod other)
        {
            return Name == other?.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptMethod) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}