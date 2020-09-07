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
                    OnVariableStatement(node);
                    _index++;
                    return;
                case SyntaxKind.ExpressionStatement:
                    OnExpressionStatement(node);
                    _index++;
                    return;
                case SyntaxKind.FunctionDeclaration:
                    OnFunctionDeclaration(node);
                    _index++;
                    return;
                case SyntaxKind.ClassDeclaration:
                    OnClassDeclaration(node);
                    _index++;
                    return;
                case SyntaxKind.InterfaceDeclaration:
                    OnInterfaceDeclaration(node);
                    _index++;
                    return;
                case SyntaxKind.Constructor:
                    OnConstructor(node);
                    _index++;
                    return;
                case SyntaxKind.MethodDeclaration:
                    OnMethodDeclaration(node);
                    _index++;
                    return;
                case SyntaxKind.PropertyDeclaration:
                    OnPropertyDeclaration(node);
                    _index++;
                    return;
                case SyntaxKind.GetAccessor:
                    OnGetAccessor(node);
                    _index++;
                    return;
                case SyntaxKind.SetAccessor:
                    OnSetAccessor(node);
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
            var existing = _editor.File.Imports.SingleOrDefault(x => x.Identifier == x.GetIdentifier(node) && !x.Types.Except(x.GetTypes(node)).Any());
            if (existing == null)
            {
                _node.InsertChild(_index, new TypeScriptFileImport(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptConstructor(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptProperty(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptGetAccessor(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptSetAccessor(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptMethod(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptVariableStatement(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptFunctionDeclaration(node, _editor));
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
                _node.InsertChild(_index, new TypeScriptExpressionStatement(node, _editor));
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
                _node.InsertChild(_index, existing);
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
                _node.InsertChild(_index, existing);
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