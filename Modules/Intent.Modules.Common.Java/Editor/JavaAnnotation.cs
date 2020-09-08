using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaAnnotation : JavaNode
    {
        public JavaAnnotation(Java9Parser.AnnotationContext context, JavaNode parent) : base(context, parent)
        {
        }

        protected override string GetIdentifier(ParserRuleContext context)
        {
            return ((Java9Parser.AnnotationContext)context).GetText();
        }
    }
}