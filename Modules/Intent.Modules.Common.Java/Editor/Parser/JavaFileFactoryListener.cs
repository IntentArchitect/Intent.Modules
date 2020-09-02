using Antlr4.Runtime.Misc;
using System.Net.Mime;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public class JavaFileFactoryListener : Java9BaseListener
    {
        public JavaFileFactoryListener(JavaFile javaFile)
        {
            File = javaFile;
        }

        public readonly JavaFile File;

        private JavaClass _currentClass;
        public override void EnterClassDeclaration(Java9Parser.ClassDeclarationContext context)
        {
            _currentClass = new JavaClass(context, File);
            File.Classes.Add(_currentClass);
        }

        public override void EnterImportDeclaration([NotNull] Java9Parser.ImportDeclarationContext context)
        {
            File.Imports.Add(new JavaImport(context));
        }

        public override void EnterClassMemberDeclaration([NotNull] Java9Parser.ClassMemberDeclarationContext context)
        {
            var methodDeclaration = context.methodDeclaration();
            if (methodDeclaration != null)
            {
                _currentClass.Children.Add(new JavaMethod(methodDeclaration, _currentClass));
            }
        }
    }
}