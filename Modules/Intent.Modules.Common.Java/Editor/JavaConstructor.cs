using System.Collections.Generic;
using System.Linq;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaConstructor : JavaNode
    {
        public JavaClass Parent { get; }
        private readonly Java9Parser.ConstructorDeclarationContext _context;

        public JavaConstructor(Java9Parser.ConstructorDeclarationContext context, JavaClass parent) : base(context, parent.File)
        {
            Parent = parent;
            _context = context;

            Name = _context.constructorDeclarator().formalParameterList() != null
                ? string.Join(", ", (_context.constructorDeclarator().formalParameterList().formalParameters()?.formalParameter()
                    .Select(x => x.unannType().GetText()) ?? new List<string>())
                    .Concat(new []{ _context.constructorDeclarator().formalParameterList().lastFormalParameter().formalParameter().unannType().GetText() }))
                : "";
            Identifier = Name; // plus parameter types
        }

        public string Name { get; }
        public override string Identifier { get; }

        public override bool IsIgnored()
        {
            var isIgnored = _context.constructorModifier().Any(x => x.annotation()?.GetText().StartsWith("@IntentIgnore") ?? false); ;
            return isIgnored;
        }
    }
}