using System.Linq;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Angular.Editor
{
    public class TypescriptClassDecorator : TypescriptNode
    {
        public TypescriptClassDecorator(Node node, TypescriptFile file) : base(node, file)
        {
        }

        public string Name => Node.First.IdentifierStr;

        public void AddProviderIfNotExists(string className)
        {
            var change = new ChangeAST();
            var providers = FindNode("PropertyAssignment:providers/ArrayLiteralExpression");
            if (providers != null)
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
            File.UpdateChanges(change);
        }

        public void AddDeclarationIfNotExists(string className)
        {
            var change = new ChangeAST();
            var declarations = FindNode("PropertyAssignment:declarations/ArrayLiteralExpression");
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
            File.UpdateChanges(change);
        }

        public void AddImportIfNotExists(string className)
        {
            var change = new ChangeAST();
            var declarations = FindNode("PropertyAssignment:imports/ArrayLiteralExpression");
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
            File.UpdateChanges(change);
        }
    }
}