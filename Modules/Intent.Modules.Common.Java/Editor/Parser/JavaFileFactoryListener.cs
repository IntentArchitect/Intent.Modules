using Antlr4.Runtime.Misc;
using System.Net.Mime;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public class JavaFileFactoryListener : Java9BaseListener
    {

        public JavaFileFactoryListener()
        {
            JavaFile = new JavaFile();
        }

        public readonly JavaFile JavaFile;

        private JavaClass _currentClass;
        public override void EnterClassDeclaration(Java9Parser.ClassDeclarationContext context)
        {
            _currentClass = new JavaClass(context);
            JavaFile.Classes.Add(_currentClass);
        }

        public override void EnterClassMemberDeclaration([NotNull] Java9Parser.ClassMemberDeclarationContext context)
        {
            var methodDeclaration = context.methodDeclaration();
            if (methodDeclaration != null)
            {
                _currentClass.Methods.Add(new JavaMethod(methodDeclaration));
            }
        }

        /*public override void EnterMethodDeclaration(Java9Parser.MethodDeclarationContext context)
        {
            base.EnterMethodDeclaration(context);
            var identifier = context.Identifier();
        }*/

        public override void ExitMethodBody(Java9Parser.MethodBodyContext context)
        {
            base.ExitMethodBody(context);
        }

        //public override void ExitMethodBody([NotNull] JavaParser.MethodBodyContext context)
        //{
        //    var ignoreAnnotate = context.Parent.Parent.Parent.GetChild(0).GetChild(0).GetText();
        //    if (ignoreAnnotate == "@Ignore")
        //    {
        //        var returnTypeStr = context.Parent.Parent.GetChild(0).GetText();
        //        var name = context.Parent.Parent.GetChild(1).GetText();
        //        var paramList = ((ParserRuleContext)context.Parent.Parent.GetChild(2).GetChild(0))
        //            .children
        //            .Where(p => p.ChildCount > 0)
        //            .Select(s => string.Join(" ", ((ParserRuleContext)s).children.Select(t => t.GetText())))
        //            .ToArray();
        //        _methods.Add(new MethodSignatureKey(returnTypeStr, name, paramList), context);
        //    }
        //}
    }
}