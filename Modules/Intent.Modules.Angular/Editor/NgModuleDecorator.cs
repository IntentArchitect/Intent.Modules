using System;
using System.Linq;
using Intent.Modules.Common.TypeScript.Editor;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public static class TypescriptClassDecoratorExtensions
    {
        public static NgModuleDecorator ToNgModule(this TypeScriptDecorator decorator)
        {
            return decorator.Name != "NgModule" ? null : new NgModuleDecorator(decorator);
        }
    }

    public class NgModuleDecorator
    {
        private readonly TypeScriptDecorator _decorator;

        public NgModuleDecorator(TypeScriptDecorator decorator)
        {
            if (decorator.Name != "NgModule")
            {
                throw new Exception($"Cannot create {nameof(NgModuleDecorator)} for underlying {nameof(TypescriptClassDecorator)} \"{decorator.Name}\"");
            }
            _decorator = decorator;
        }

        public void AddProviderIfNotExists(string className)
        {
            var providers = _decorator.FindNode("PropertyAssignment:providers/ArrayLiteralExpression");
            if (providers == null)
            {
                var parameter = _decorator.Parameters().First();
                parameter.InsertPropertyAssignment($@"
  providers: [
    {className}
  ]", _decorator.FindNode("PropertyAssignment:declarations"));
            }
            else
            {
                if (providers.Children.Count == 0)
                {
                    _decorator.Change.ChangeNode(providers, $@" [
    {className}
  ]");
                }
                else if (providers.Children.All(x => x.IdentifierStr != className))
                {
                    _decorator.Change.InsertAfter(providers.Children.Last(), $@", 
    {className}");
                }
            }
        }

        public void AddDeclarationIfNotExists(string className)
        {
            var declarations = _decorator.FindNode("PropertyAssignment:declarations/ArrayLiteralExpression");
            if (declarations == null)
            {
                var parameter = _decorator.Parameters().First();
                parameter.InsertPropertyAssignment($@"
  declarations: [
    {className}
  ]", _decorator.FindNode($"ObjectLiteralExpression"));
            }
            else
            {
                if (declarations.Children.Count == 0)
                {
                    _decorator.Change.ChangeNode(declarations, $@" [
    {className}
  ]");
                }
                else if (declarations.Children.All(x => x.IdentifierStr != className))
                {
                    _decorator.Change.InsertAfter(declarations.Children.Last(), $@", 
    {className}");
                }
            }
        }

        public void AddImportIfNotExists(string className)
        {
            var declarations = _decorator.FindNode("PropertyAssignment:imports/ArrayLiteralExpression");
            if (declarations != null)
            {
                if (declarations.Children.Count == 0)
                {
                    _decorator.Change.ChangeNode(declarations, $@" [
    {className}
  ]");
                }
                else if (declarations.Children.All(x => x.IdentifierStr != className))
                {
                    _decorator.Change.InsertAfter(declarations.Children.Last(), $@", 
    {className}");
                }
            }
        }
    }
}