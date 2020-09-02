using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptInterface : TypeScriptNode, IEquatable<TypeScriptInterface>
    {
        public TypeScriptInterface(Node node, TypeScriptFile file) : base(node, file)
        {
        }

        public string Name => Node.IdentifierStr;

        private IList<TypeScriptProperty> _properties;
        public IList<TypeScriptProperty> Properties()
        {
            return _properties ?? (_properties = Node.OfKind(SyntaxKind.PropertyDeclaration).Select(x => new TypeScriptProperty(x, File)).ToList());
        }

        private IList<TypeScriptMethod> _methods;
        public IList<TypeScriptMethod> Methods()
        {
            return _methods ?? (_methods = Node.OfKind(SyntaxKind.MethodDeclaration).Select(x => new TypeScriptMethod(x, File)).ToList());
        }

        public bool Equals(TypeScriptInterface other)
        {
            return Name == other?.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptInterface)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}