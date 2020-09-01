using System;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptVariableStatement : TypeScriptNode, IEquatable<TypeScriptVariableStatement>
    {
        public TypeScriptVariableStatement(Node node, TypeScriptFile file) : base(node, file)
        {
            Name = Node.OfKind(SyntaxKind.VariableDeclaration).First().IdentifierStr ?? throw new ArgumentException("Variable Name could not be determined for node: " + this);
        }

        public string Name { get; }

        public T GetAssignedValue<T>()
            where T : TypeScriptNode
        {
            var arrayLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ArrayLiteralExpression);
            if (arrayLiteral != null)
            {
                return new TypeScriptArrayLiteralExpression(arrayLiteral, File) as T;
            }
            var objectLiteral = Node.Children.FirstOrDefault(x => x.Kind == SyntaxKind.ObjectLiteralExpression);
            if (objectLiteral != null)
            {
                return new TypeScriptObjectLiteralExpression(objectLiteral, File) as T;
            }
            // TODO: ValueLiteral

            return null;
        }

        public bool Equals(TypeScriptVariableStatement other)
        {
            return Name != other?.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptVariableStatement) obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}