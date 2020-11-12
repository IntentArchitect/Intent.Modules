using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaConstructor : JavaNode
    {
        private readonly JavaParser.ConstructorDeclarationContext _context;

        public JavaConstructor(JavaParser.ConstructorDeclarationContext context, JavaClass parent) : base(context, parent)
        {
            _context = context;
        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return ((JavaParser.ConstructorDeclarationContext)context).formalParameters().formalParameterList() != null
                ? string.Join(", ", ((JavaParser.ConstructorDeclarationContext)context).formalParameters().formalParameterList().GetParameterTypes())
                : "";
        }

        //public override bool IsIgnored()
        //{
        //    var isIgnored = _context.constructorModifier().Any(x => x.annotation()?.GetText().StartsWith("@IntentIgnore") ?? false); ;
        //    return isIgnored;
        //}
    }
}