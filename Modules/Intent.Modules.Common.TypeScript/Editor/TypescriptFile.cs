using System.Collections.Generic;
using System.Linq;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor
{
    public class TypeScriptFile
    {
        private string _source;
        public TypeScriptAST Ast;

        public ChangeAST Change;

        private IList<TypeScriptNode> _registeredNodes = new List<TypeScriptNode>();

        public TypeScriptFile(string source)
        {
            _source = source;
            Ast = new TypeScriptAST(_source);
            Change = new ChangeAST();
        }

        private List<TypescriptFileImport> _imports;
        public List<TypescriptFileImport> Imports()
        {
            return _imports ?? (_imports = Ast.OfKind(SyntaxKind.ImportDeclaration).Select(x => new TypescriptFileImport(x, this)).ToList());
        }

        public bool ImportExists(TypescriptFileImport import)
        {
            return import.Types.All(type => Imports().Exists(x => x.HasType(type) && x.Location == import.Location));
        }

        public void AddImport(TypescriptFileImport import)
        {
            if (Imports().Any())
            {
                var existingLocation = Imports().FirstOrDefault(x => x.Location == import.Location);
                if (existingLocation != null)
                {
                    foreach (var importType in import.Types)
                    {
                        if (!existingLocation.HasType(importType))
                        {
                            existingLocation.AddType(importType);
                        }
                    }
                }
                else
                {
                    Change.InsertAfter(Imports().Last().Node, $@"{import.GetTextWithComments()}");
                }
            }
            else
            {
                Change.InsertBefore(Ast.RootNode, $@"{import.GetTextWithComments()}");
            }
            UpdateChanges();
        }

        public void AddImportIfNotExists(string className, string location)
        {
            var imports = Ast.OfKind(SyntaxKind.ImportDeclaration);

            if (imports.Any())
            {
                if (imports.All(x => x.GetDescendants(false).OfKind(SyntaxKind.Identifier).FirstOrDefault()?.IdentifierStr != className))
                {
                    Change.InsertAfter(imports.Last(), $@"
import {{ {className} }} from '{location}';");
                }
            }
            else
            {
                Change.InsertBefore(Ast.RootNode, $@"
import {{ {className} }} from '{location}';");
            }
            UpdateChanges();
        }

        public IList<TypeScriptVariableDeclaration> VariableDeclarations()
        {
            return Ast.OfKind(SyntaxKind.VariableStatement).Select(x => new TypeScriptVariableDeclaration(x, this)).ToList();
        }

        public IList<TypeScriptClass> ClassDeclarations()
        {
            return Ast.OfKind(SyntaxKind.ClassDeclaration).Select(x => new TypeScriptClass(x, this)).ToList();
        }

        public IList<TypeScriptClass> InterfaceDeclarations()
        {
            return Ast.OfKind(SyntaxKind.InterfaceDeclaration).Select(x => new TypeScriptClass(x, this)).ToList();
        }

        public void AddVariableDeclaration(string declaration)
        {
            var variables = Ast.OfKind(SyntaxKind.VariableStatement);
            Change.InsertAfter(variables.Any() ? variables.Last() : Ast.RootNode.Children.Last(), declaration);
            UpdateChanges();
        }

        public void AddClass(string declaration)
        {
            var classes = Ast.OfKind(SyntaxKind.ClassDeclaration);
            Change.InsertAfter(classes.Any() ? classes.Last() : Ast.RootNode.Children.Last(), declaration);
            UpdateChanges();
        }

        public void AddInterface(string declaration)
        {
            var interfaces = Ast.OfKind(SyntaxKind.InterfaceDeclaration);
            Change.InsertAfter(interfaces.Any() ? interfaces.Last() : Ast.RootNode.Children.Last(), declaration);
            UpdateChanges();
        }

        public void ReplaceNode(Node node, string replaceWith = "")
        {
            _source = _source
                .Remove(node.Pos.Value, node.End.Value - node.Pos.Value)
                .Insert(node.Pos.Value, replaceWith);
            //Ast = new TypeScriptAST(_source);
            //Change = new ChangeAST();
            //foreach (var typeScriptNode in _registeredNodes)
            //{
            //    typeScriptNode.UpdateNode();
            //}
        }

        public string GetSource()
        {
            return _source;
        }

        public void UpdateChanges()
        {
            _source = Change.GetChangedSource(_source);
            Ast = new TypeScriptAST(_source);
            Change = new ChangeAST();
            foreach (var node in _registeredNodes)
            {
                node.UpdateNode();
            }
        }

        public void Register(TypeScriptNode node)
        {
            _registeredNodes.Add(node);
        }

        public void Unregister(TypeScriptNode node)
        {
            _registeredNodes.Remove(node);
        }

        public override string ToString()
        {
            return _source;
        }
    }
}