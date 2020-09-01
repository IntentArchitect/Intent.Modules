using System;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaMethod
    {
        private readonly Java9Parser.MethodDeclarationContext _context;

        public JavaMethod(Java9Parser.MethodDeclarationContext context)
        {
            _context = context;
            Name = _context.methodHeader().methodDeclarator().identifier().GetText();
        }

        public string Name { get; }
    }
}