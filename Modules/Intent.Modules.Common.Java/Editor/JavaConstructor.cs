using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaConstructor : JavaNode
    {
        private readonly Java9Parser.ConstructorDeclarationContext _context;

        public JavaConstructor(Java9Parser.ConstructorDeclarationContext context, JavaClass parent) : base(context, parent)
        {
            _context = context;
        }

        protected override string GetIdentifier(ParserRuleContext context)
        {
            return ((Java9Parser.ConstructorDeclarationContext)context).constructorDeclarator().formalParameterList() != null
                ? string.Join(", ", (((Java9Parser.ConstructorDeclarationContext)context).constructorDeclarator().formalParameterList().formalParameters()?.formalParameter()
                        .Select(x => x.unannType().GetText()) ?? new List<string>())
                    .Concat(new[] { ((Java9Parser.ConstructorDeclarationContext)context).constructorDeclarator().formalParameterList().lastFormalParameter().formalParameter().unannType().GetText() }))
                : "";
        }

        public override bool IsIgnored()
        {
            var isIgnored = _context.constructorModifier().Any(x => x.annotation()?.GetText().StartsWith("@IntentIgnore") ?? false); ;
            return isIgnored;
        }
    }
}