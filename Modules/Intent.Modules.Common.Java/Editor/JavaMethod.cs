using System;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaMethod
    {
        private readonly JavaParser.MethodDeclarationContext _context;

        public JavaMethod(JavaParser.MethodDeclarationContext context)
        {
            _context = context;
            Name = _context.Identifier().GetText();
        }

        public string Name { get; }
    }
}