using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaSuperClass : JavaNode
    {
        public JavaSuperClass(JavaParser.TypeTypeContext context, JavaNode parent) : base(context, parent)
        {

        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return context.GetText();
        }

        public override IToken StartToken => File.GetPreviousWsToken(((JavaParser.ClassDeclarationContext)Context.Parent).EXTENDS().Symbol);
    }
}