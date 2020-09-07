using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptInterface : TypeScriptNode
    {
        public TypeScriptInterface(Node node, TypeScriptFileEditor editor) : base(node, editor)
        {
        }

        //public string Name => Node.IdentifierStr;

        public override string GetIdentifier(Node node)
        {
            return node.IdentifierStr;
        }

        //private IList<TypeScriptProperty> _properties;

        //public IList<TypeScriptProperty> Properties()
        //{
        //    return _properties ?? (_properties = Node.OfKind(SyntaxKind.PropertyDeclaration).Select(x => new TypeScriptProperty(x, File)).ToList());
        //}

        //private IList<TypeScriptMethod> _methods;

        //public IList<TypeScriptMethod> Methods()
        //{
        //    return _methods ?? (_methods = Node.OfKind(SyntaxKind.MethodDeclaration).Select(x => new TypeScriptMethod(x, File)).ToList());
        //}
    }
}