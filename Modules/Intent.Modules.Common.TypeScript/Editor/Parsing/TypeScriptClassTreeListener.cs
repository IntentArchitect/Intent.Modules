using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.TypeScript.Editor.Parsing
{
    public class TypeScriptClassTreeListener
    {
        private readonly TypeScriptClass _class;

        public TypeScriptClassTreeListener(TypeScriptClass @class)
        {
            _class = @class;
        }

        public void WalkTree()
        {
            WalkTree(_class.Node);
        }

        private void WalkTree(Node node)
        {
            switch (node.Kind)
            {
                case SyntaxKind.ImportDeclaration:
                    OnImportDeclaration(node);
                    return;
                case SyntaxKind.VariableStatement:
                    OnImportDeclaration(node);
                    return;
                case SyntaxKind.ExpressionStatement:
                    OnImportDeclaration(node);
                    return;
                case SyntaxKind.ClassDeclaration:
                    OnImportDeclaration(node);
                    return;
            }
            foreach (var child in node.Children)
            {
                WalkTree(child);
            }
        }

        private void OnImportDeclaration(Node node)
        {
            throw new System.NotImplementedException();
        }
    }
}