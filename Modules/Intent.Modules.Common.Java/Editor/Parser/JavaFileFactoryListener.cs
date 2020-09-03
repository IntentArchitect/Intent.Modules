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
            File.Imports.Add(new JavaImport(context, File));
        }

        public override void EnterConstructorDeclaration(Java9Parser.ConstructorDeclarationContext context)
        {
            _currentClass.Children.Add(new JavaConstructor(context, _currentClass));
        }

        public override void EnterClassMemberDeclaration([NotNull] Java9Parser.ClassMemberDeclarationContext context)
        {
            var methodDeclaration = context.methodDeclaration();
            if (methodDeclaration != null)
            {
                _currentClass.Children.Add(new JavaMethod(methodDeclaration, _currentClass));
            }

            var fieldDeclaration = context.fieldDeclaration();
            if (fieldDeclaration != null)
            {
                _currentClass.Children.Add(new JavaField(fieldDeclaration, _currentClass));
            }
        }
    }
}