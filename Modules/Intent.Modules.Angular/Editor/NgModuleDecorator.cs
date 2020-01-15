using System;
using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public static class TypescriptClassDecoratorExtensions
    {
        public static NgModuleDecorator ToNgModule(this TypescriptClassDecorator decorator)
        {
            return decorator.Name != "NgModule" ? null : new NgModuleDecorator(decorator);
        }
    }

    public class NgModuleDecorator
    {
        private readonly TypescriptClassDecorator _decorator;

        public NgModuleDecorator(TypescriptClassDecorator decorator)
        {
            if (decorator.Name != "NgModule")
            {
                throw new Exception($"Cannot create {nameof(NgModuleDecorator)} for underlying {nameof(TypescriptClassDecorator)} \"{decorator.Name}\"");
            }
            _decorator = decorator;
        }

        public void AddProviderIfNotExists(string className)
        {
            var change = new ChangeAST();
            var providers = _decorator.FindNode("PropertyAssignment:providers/ArrayLiteralExpression");
            if (providers == null)
            {
                var parameter = _decorator.Parameters().First();
                parameter.InsertPropertyAssignment($@",
  providers: [
    {className}
  ]", _decorator.FindNode("PropertyAssignment:declarations/ArrayLiteralExpression"));
            }
            else
            {
                if (providers.Children.Count == 0)
                {
                    change.ChangeNode(providers, $@" [
    {className}
  ]");
                }
                else if (providers.Children.All(x => x.IdentifierStr != className))
                {
                    change.InsertAfter(providers.Children.Last(), $@", 
    {className}");
                }
            }
            _decorator.UpdateChanges(change);
        }

        public void AddDeclarationIfNotExists(string className)
        {
            var change = new ChangeAST();
            var declarations = _decorator.FindNode("PropertyAssignment:declarations/ArrayLiteralExpression");
            if (declarations == null)
            {
                var parameter = _decorator.Parameters().First();
                parameter.InsertPropertyAssignment($@",
  declarations: [
    {className}
  ]", _decorator.FindNode($"ObjectLiteralExpression"));
            }
            else
            {
                if (declarations.Children.Count == 0)
                {
                    change.ChangeNode(declarations, $@" [
    {className}
  ]");
                }
                else if (declarations.Children.All(x => x.IdentifierStr != className))
                {
                    change.InsertAfter(declarations.Children.Last(), $@", 
    {className}");
                }
            }
            _decorator.UpdateChanges(change);
        }

        public void AddImportIfNotExists(string className)
        {
            var change = new ChangeAST();
            var declarations = _decorator.FindNode("PropertyAssignment:imports/ArrayLiteralExpression");
            if (declarations != null)
            {
                if (declarations.Children.Count == 0)
                {
                    change.ChangeNode(declarations, $@" [
    {className}
  ]");
                }
                else if (declarations.Children.All(x => x.IdentifierStr != className))
                {
                    change.InsertAfter(declarations.Children.Last(), $@", 
    {className}");
                }
            }
            _decorator.UpdateChanges(change);
        }
    }
}