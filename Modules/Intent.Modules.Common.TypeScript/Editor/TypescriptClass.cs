using System;
using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptClass : TypeScriptNode, IEquatable<TypeScriptClass>
    {
        public TypeScriptClass(Node node, TypeScriptFile file) : base(node, file)
        {

        }

        public string Name => Node.IdentifierStr;

        public IList<TypeScriptDecorator> Decorators()
        {
            return Node.Decorators?.Select(x => new TypeScriptDecorator(x, File)).ToList() ?? new List<TypeScriptDecorator>();
            //return Node.OfKind(SyntaxKind.Decorator).Select(x => new TypeScriptDecorator(x, File)).ToList();
        }

        public TypeScriptDecorator GetDecorator(string name)
        {
            return Decorators().SingleOrDefault(x => x.Name == name);
        }

        public bool HasConstructor()
        {
            return Node.OfKind(SyntaxKind.Constructor).Any();
        }

        public TypeScriptConstructor Constructor()
        {
            return HasConstructor() ? new TypeScriptConstructor(Node.OfKind(SyntaxKind.Constructor).First(), File) : null;
        }

        public void AddConstructor(string text)
        {
            if (IsEmptyClass())
            {
                AddCodeToClass(text);
                return;
            }

            var methods = Node.OfKind(SyntaxKind.MethodDeclaration);
            if (methods.Any())
            {
                Change.InsertBefore(methods.First(), text);
            }

            var properties = Node.OfKind(SyntaxKind.PropertyDeclaration);
            Change.InsertAfter(properties.LastOrDefault() ?? Node.Children.Last(), text);

            UpdateChanges();
        }

        private IList<TypeScriptMethod> _methods;

        public IList<TypeScriptMethod> Methods()
        {
            return _methods ?? (_methods = Node.OfKind(SyntaxKind.MethodDeclaration).Select(x => new TypeScriptMethod(x, File)).ToList());
        }

        public bool MethodExists(string methodName)
        {
            var methods = Node.OfKind(SyntaxKind.MethodDeclaration);

            if (!methods.Any())
            {
                return false;
            }

            return NodeExists($"MethodDeclaration/MethodDeclaration:{methodName}");
        }

        public void AddMethod(string method)
        {
            if (IsEmptyClass())
            {
                AddCodeToClass(method);
                return;
            }

            var methods = Node.OfKind(SyntaxKind.MethodDeclaration);
            Change.InsertAfter(methods.Any() ? methods.Last() 
                : HasConstructor() ? Constructor().Node 
                : Node.Children.Last(), method);

            UpdateChanges();
        }

        public void ReplaceMethod(string methodName, string method)
        {
            var existing = Methods().FirstOrDefault(x => x.Name == methodName);

            if (existing == null)
            {
                throw new InvalidOperationException($"Method ({methodName}) could not be found.");
            }

            existing.ReplaceWith(method);
        }

        private IList<TypeScriptProperty> _properties;

        public IList<TypeScriptProperty> Properties()
        {
            return _properties ?? (_properties = Node.OfKind(SyntaxKind.PropertyDeclaration).Select(x => new TypeScriptProperty(x, File)).ToList());
        }

        public bool PropertyExists(string propertyName)
        {
            var properties = Node.OfKind(SyntaxKind.PropertyDeclaration);

            return properties.Any(x => x.IdentifierStr == propertyName);
        }

        public void AddProperty(string propertyDeclaration)
        {
            if (IsEmptyClass())
            {
                AddCodeToClass(propertyDeclaration);
                return;
            }

            var properties = Node.OfKind(SyntaxKind.PropertyDeclaration);

            if (properties.Any())
            {
                Change.InsertAfter(properties.Last(), propertyDeclaration);
            }
            else
            {
                Change.InsertBefore(Node.Children.OfKind(SyntaxKind.Constructor).FirstOrDefault() ?? Node.Children.OfKind(SyntaxKind.MethodDeclaration).FirstOrDefault() ?? Node.Children.Last(), propertyDeclaration);
            }

            UpdateChanges();
        }

        public bool IsEmptyClass()
        {
            return Node.Last.Kind == SyntaxKind.Identifier && Node.Last.IdentifierStr == Name;
        }

        public void AddCodeToClass(string code)
        {
            var overwriteClass = Node.GetTextWithComments();
            overwriteClass = overwriteClass.Insert(overwriteClass.LastIndexOf('{') + 1, code);
            File.ReplaceNode(Node, overwriteClass);
            UpdateChanges();
        }

        public override void UpdateChanges()
        {
            if (_methods != null)
            {
                foreach (var method in _methods)
                {
                    File.Unregister(method);
                }
                _methods = null;
            }

            base.UpdateChanges();
        }

        public override bool IsIgnored()
        {
            return GetDecorator("IntentIgnore") != null;
        }

        public bool IsMerged()
        {
            return GetDecorator("IntentMerge") != null;
        }

        public bool Equals(TypeScriptClass other)
        {
            return Name == other?.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TypeScriptClass)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}