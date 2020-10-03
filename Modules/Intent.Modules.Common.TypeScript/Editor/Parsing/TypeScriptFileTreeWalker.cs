using System;
using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor.Parsing
{
    public class TypeScriptFileTreeWalker
    {
        private readonly TypeScriptFileEditor _editor;
        private readonly TypeScriptNode _node;
        private int _index = 0;

        public TypeScriptFileTreeWalker(TypeScriptNode node, TypeScriptFileEditor editor)
        {
            _editor = editor;
            _node = node;
        }

        public void WalkTree()
        {
            foreach (var child in _node.Node.Children)
            {
                WalkTree(child);
            }
        }

        private void WalkTree(Node node)
        {
            switch (node.Kind)
            {
                case SyntaxKind.ImportDeclaration:
                    OnImportDeclaration(node);
                    _index++;
                    return;
                case SyntaxKind.VariableStatement:
                    InsertOrUpdateNode(node, CreateVariableStatement);
                    _index++;
                    return;
                case SyntaxKind.ExpressionStatement:
                    InsertOrUpdateNode(node, CreateExpressionStatement);
                    _index++;
                    return;
                case SyntaxKind.FunctionDeclaration:
                    InsertOrUpdateNode(node, CreateFunctionDeclaration);
                    _index++;
                    return;
                case SyntaxKind.ClassDeclaration:
                    InsertOrUpdateNode(node, CreateClass);
                    _index++;
                    return;
                case SyntaxKind.InterfaceDeclaration:
                    InsertOrUpdateNode(node, CreateInterface);
                    _index++;
                    return;
                case SyntaxKind.Constructor:
                    InsertOrUpdateNode(node, CreateConstructor);
                    _index++;
                    return;
                case SyntaxKind.MethodDeclaration:
                    InsertOrUpdateNode(node, CreateMethod);
                    _index++;
                    return;
                case SyntaxKind.PropertyDeclaration:
                    InsertOrUpdateNode(node, CreatePropertyDeclaration);
                    _index++;
                    return;
                case SyntaxKind.GetAccessor:
                    InsertOrUpdateNode(node, CreateGetAccessor);
                    _index++;
                    return;
                case SyntaxKind.SetAccessor:
                    InsertOrUpdateNode(node, CreateSetAccessor);
                    _index++;
                    return;
            }
            foreach (var child in node.Children)
            {
                WalkTree(child);
            }
        }

        private void OnImportDeclaration(Node node)
        {
            var existing = _editor.File.Imports.SingleOrDefault(x => x.Identifier == x.GetIdentifier(node) && x.Types.OrderBy(t => t).SequenceEqual(x.GetTypes(node).OrderBy(t => t)));
            if (existing == null)
            {
                _node.InsertChild(_index, new TypeScriptFileImport(node, _node));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        protected virtual TypeScriptNode CreateClass(Node node, TypeScriptNode parent)
        {
            return new TypeScriptClass(node, parent);
        }

        protected virtual TypeScriptNode CreateInterface(Node node, TypeScriptNode parent)
        {
            return new TypeScriptInterface(node, parent);
        }

        protected virtual TypeScriptNode CreateConstructor(Node node, TypeScriptNode parent)
        {
            return new TypeScriptConstructor(node, parent);
        }

        protected virtual TypeScriptNode CreatePropertyDeclaration(Node node, TypeScriptNode parent)
        {
            return new TypeScriptProperty(node, parent);
        }

        protected virtual TypeScriptNode CreateGetAccessor(Node node, TypeScriptNode parent)
        {
            return new TypeScriptGetAccessor(node, parent);
        }

        protected virtual TypeScriptNode CreateSetAccessor(Node node, TypeScriptNode parent)
        {
            return new TypeScriptSetAccessor(node, parent);
        }

        protected virtual TypeScriptNode CreateMethod(Node node, TypeScriptNode parent)
        {
            return new TypeScriptMethod(node, parent);
        }

        protected virtual TypeScriptNode CreateVariableStatement(Node node, TypeScriptNode parent)
        {
            return new TypeScriptVariableStatement(node, parent);
        }

        protected virtual TypeScriptNode CreateFunctionDeclaration(Node node, TypeScriptNode parent)
        {
            return new TypeScriptFunctionDeclaration(node, parent);
        }

        protected virtual TypeScriptNode CreateExpressionStatement(Node node, TypeScriptNode parent)
        {
            return new TypeScriptExpressionStatement(node, parent);
        }

        protected void InsertOrUpdateNode(Node node, Func<Node, TypeScriptNode, TypeScriptNode> create)
        {
            _node.InsertOrUpdateChildNode(node, _index, () => create(node, _node));
        }
    }
}