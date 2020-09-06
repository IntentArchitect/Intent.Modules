using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor.Parsing
{
    public class TypeScriptFileTreeListener
    {
        private readonly TypeScriptFile _file;

        public TypeScriptFileTreeListener(TypeScriptFile file)
        {
            _file = file;
        }

        public void WalkTree()
        {
            WalkTree(_file.Ast.RootNode);
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
                case SyntaxKind.ClassDeclaration:
                    OnClassDeclaration(node);
                    return;
            }
            foreach (var child in node.Children)
            {
                WalkTree(child);
            }
        }

        private void OnImportDeclaration(Node node)
        {
            var existing = _file.TryGetChild(node);
            if (existing == null)
            {
                _file.Children.Add(new TypeScriptFileImport(node, _file));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnVariableStatement(Node node)
        {
            var existing = _file.TryGetChild(node);
            if (existing == null)
            {
                _file.Children.Add(new TypeScriptVariableStatement(node, _file));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnExpressionStatement(Node node)
        {
            var existing = _file.TryGetChild(node);
            if (existing == null)
            {
                _file.Children.Add(new TypeScriptExpressionStatement(node, _file));
            }
            else
            {
                existing.UpdateNode(node);
            }
        }

        private void OnClassDeclaration(Node node)
        {
            var existing = _file.TryGetChild(node);
            if (existing == null)
            {
                existing = new TypeScriptClass(node, _file);
                _file.Children.Add(existing);
            }
            else
            {
                existing.UpdateNode(node);
            }
            var listener = new TypeScriptClassTreeListener((TypeScriptClass)existing);
            listener.WalkTree();
        }
    }
}