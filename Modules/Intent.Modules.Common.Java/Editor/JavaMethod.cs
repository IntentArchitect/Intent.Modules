using System;
using System.Linq;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaMethod : JavaNode
    {
        public JavaClass Parent { get; }
        private readonly Java9Parser.MethodDeclarationContext _context;

        public JavaMethod(Java9Parser.MethodDeclarationContext context, JavaClass parent) : base(context, parent.File)
        {
            Parent = parent;
            _context = context;
            Name = _context.methodHeader().methodDeclarator().identifier().GetText();
            Identifier = Name; // plus parameter types
        }

        public string Name { get; }
        public override string Identifier { get; }

        public override bool IsIgnored()
        {
            var isIgnored = _context.methodModifier().Any(x => x.annotation()?.GetText().StartsWith("@IntentIgnore") ?? false);;
            return isIgnored;
        }
    }
}