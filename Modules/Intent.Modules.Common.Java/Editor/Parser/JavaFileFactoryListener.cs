using System.Linq;
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
            if (!File.HasChild(_currentClass))
            {
                File.AddChild(_currentClass);
            }
            else
            {
                _currentClass = (JavaClass) File.TryGetChild(context);
                _currentClass.UpdateContext(context);
                //_currentClass = (JavaClass)File.Children.Single(x => x.Equals(_currentClass));
            }
        }

        public override void EnterImportDeclaration([NotNull] Java9Parser.ImportDeclarationContext context)
        {
            var import = new JavaImport(context, File);
            if (!File.ImportExists(import))
            {
                File.Imports.Add(import);
            }
        }

        public override void EnterConstructorDeclaration(Java9Parser.ConstructorDeclarationContext context)
        {
            var constructor = new JavaConstructor(context, _currentClass);
            if (!_currentClass.HasChild(constructor))
            {
                _currentClass.AddChild(constructor);
            }
            else
            {
                _currentClass.TryGetChild(context).UpdateContext(context);
            }
        }

        public override void EnterClassMemberDeclaration([NotNull] Java9Parser.ClassMemberDeclarationContext context)
        {
            var methodDeclaration = context.methodDeclaration();
            if (methodDeclaration != null)
            {
                var method = new JavaMethod(methodDeclaration, _currentClass);
                if (!_currentClass.HasChild(method))
                {
                    _currentClass.AddChild(method);
                }
                else
                {
                    _currentClass.TryGetChild(methodDeclaration).UpdateContext(methodDeclaration);
                }
            }

            var fieldDeclaration = context.fieldDeclaration();
            if (fieldDeclaration != null)
            {
                var field = new JavaField(fieldDeclaration, _currentClass);
                if (!_currentClass.HasChild(field))
                {
                    _currentClass.AddChild(field);
                }
                else
                {
                    _currentClass.TryGetChild(fieldDeclaration).UpdateContext(fieldDeclaration);
                }
            }
        }
    }
}