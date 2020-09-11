using Antlr4.Runtime.Tree;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public class JavaClassListener : JavaNodeListener
    {
        private readonly JavaClass _node;

        public JavaClassListener(JavaClass node) : base(node)
        {
            _node = node;
        }

        public override void EnterConstructorDeclaration(Java9Parser.ConstructorDeclarationContext context)
        {
            var node = InsertOrUpdateNode(context, () => new JavaConstructor(context, _node));
            ParseTreeWalker.Default.Walk(new JavaNodeListener(node), context);
        }

        public override void EnterMethodDeclaration(Java9Parser.MethodDeclarationContext context)
        {
            var node = InsertOrUpdateNode(context, () => new JavaMethod(context, _node));
            ParseTreeWalker.Default.Walk(new JavaNodeListener(node), context);
        }

        public override void EnterFieldDeclaration(Java9Parser.FieldDeclarationContext context)
        {
            var node = InsertOrUpdateNode(context, () => new JavaField(context, _node));
            ParseTreeWalker.Default.Walk(new JavaNodeListener(node), context);
        }

        //public override void EnterClassDeclaration(Java9Parser.ClassDeclarationContext context)
        //{
        //    if (context == _node.Context)
        //    {
        //        return;
        //    }
        //    var node = InsertOrUpdateNode(context, () => new JavaClass(context, _node));
        //    ParseTreeWalker.Default.Walk(new JavaClassListener((JavaClass) node), context);
        //}
    }
}