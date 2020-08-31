using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptProperty : TypeScriptNode, IEquatable<TypeScriptProperty>
    {
        public TypeScriptProperty(Node node, TypeScriptFile file) : base(node, file)
        {

        }

        public string Name => Node.IdentifierStr;

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

        public bool IsManaged()
        {
            return GetDecorator("IntentManage") != null;
        }

        public bool Equals(TypeScriptProperty other)
        {
            return Name == other?.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptProperty)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}