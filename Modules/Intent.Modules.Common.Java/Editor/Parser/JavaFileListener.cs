using Antlr4.Runtime.Tree;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public class JavaFileListener : JavaNodeListener
    {
        private readonly JavaFile _file;

        public JavaFileListener(JavaFile file) : base(file)
        {
            _file = file;
        }

        //public override void EnterClassDeclaration(Java9Parser.ClassDeclarationContext context)
        //{
        //    var node = InsertOrUpdateNode(context, () => new JavaClass(context, _file));
        //    ParseTreeWalker.Default.Walk(new JavaClassListener((JavaClass)node), context);
        //}

        public override void EnterImportDeclaration(Java9Parser.ImportDeclarationContext context)
        {
            var import = new JavaImport(context, _file);
            if (!_file.ImportExists(import))
            {
                _file.Imports.Add(import);
            }
        }

        public override void EnterAnnotation(Java9Parser.AnnotationContext context)
        {
            // NOP
        }
    }
}