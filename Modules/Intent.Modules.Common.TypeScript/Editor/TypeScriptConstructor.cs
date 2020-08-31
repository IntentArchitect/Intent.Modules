using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptConstructor : TypeScriptNode
    {
        public TypeScriptConstructor(Node node, TypeScriptFile file) : base(node, file)
        {

        }

        public IList<TypeScriptDecorator> Decorators()
        {
            return Node.Decorators?.Select(x => new TypeScriptDecorator(x, File)).ToList() ?? new List<TypeScriptDecorator>();
        }

        public TypeScriptDecorator GetDecorator(string name)
        {
            return Decorators().SingleOrDefault(x => x.Name == name);
        }

        public override bool IsIgnored()
        {
            return GetDecorator("IntentIgnore") != null;
        }
    }
}