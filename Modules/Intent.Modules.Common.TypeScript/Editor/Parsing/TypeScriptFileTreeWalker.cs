using System.Linq;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor.Parsing
{
    public class TypeScriptFileTreeWalker
    {
        private readonly TypeScriptFileEditor _editor;
        private readonly TypeScriptNode _node;

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
                    return;
                case SyntaxKind.VariableStatement:
                    OnVariableStatement(node);
                    return;
                case SyntaxKind.ExpressionStatement:
                    OnExpressionStatement(node);
                    return;
                case SyntaxKind.FunctionDeclaration:
                    OnFunctionDeclaration(node);
                    return;
                case SyntaxKind.ClassDeclaration:
                    OnClassDeclaration(node);
                    return;
                case SyntaxKind.InterfaceDeclaration:
                    OnInterfaceDeclaration(node);
                    return;
                case SyntaxKind.Constructor:
                    OnConstructor(node);
                    return;
                case SyntaxKind.MethodDeclaration:
                    OnMethodDeclaration(node);
                    return;
                case SyntaxKind.PropertyDeclaration:
                    OnPropertyDeclaration(node);
                    return;
                case SyntaxKind.GetAccessor:
                    OnGetAccessor(node);
                    return;
                case SyntaxKind.SetAccessor:
                    OnSetAccessor(node);
                    return;
                    //case SyntaxKind.Decorator:
                    //    OnDecorator(node);
                    //    return;
            }
            foreach (var child in node.Children)
            {
                WalkTree(child);
            }
        }

        private void OnImportDeclaration(Node node)
        {
            var existing = _editor.File.Imports.SingleOrDefault(x => x.Identifier == x.GetIdentifier(node));
            if (existing == null)
            {
                _editor.File.Imports.Add(new TypeScriptFileImport(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnConstructor(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptConstructor(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnPropertyDeclaration(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptProperty(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnGetAccessor(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptGetAccessor(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnSetAccessor(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptSetAccessor(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnMethodDeclaration(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptMethod(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnVariableStatement(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptVariableStatement(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnFunctionDeclaration(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptFunctionDeclaration(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnExpressionStatement(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                _node.Children.Add(new TypeScriptExpressionStatement(node, _editor));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnDecorator(Node node)
        {
            var existing = _node.Decorators.SingleOrDefault(x => x.Identifier == x.GetIdentifier(node));
            if (existing == null)
            {
                _node.Decorators.Add(new TypeScriptDecorator(node, _node));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnClassDeclaration(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                existing = new TypeScriptClass(node, _editor);
                _node.Children.Add(existing);
            }
            else
            {
                existing.UpdateNode(node);
            }
            var listener = new TypeScriptFileTreeWalker(existing, _editor);
            listener.WalkTree();
        }

        private void OnInterfaceDeclaration(Node node)
        {
            var existing = _node.TryGetChild(node);
            if (existing == null)
            {
                existing = new TypeScriptInterface(node, _editor);
                _node.Children.Add(existing);
            }
            else
            {
                existing.UpdateNode(node);
            }
            var listener = new TypeScriptFileTreeWalker(existing, _editor);
            listener.WalkTree();
        }
    }
}