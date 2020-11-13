using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaAnnotation : JavaNode
    {
        public JavaAnnotation(JavaParser.AnnotationContext context, JavaNode parent) : base(context, parent)
        {
        }

        public override IToken StartToken => File.GetPreviousToken(Context.Start).Type == JavaLexer.WS ? File.GetPreviousToken(Context.Start) : Context.Start;

        public override string GetIdentifier(ParserRuleContext context)
        {
            return ((JavaParser.AnnotationContext)context).GetText();
        }
    }
}