using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaImport : JavaNode
    {
        private readonly JavaParser.ImportDeclarationContext _context;

        public JavaImport(JavaParser.ImportDeclarationContext context, JavaFile file) : base(context, file)
        {
            _context = context;

            // We can also split this up into different constructors and have different methods
            // overridden in the listener according to each import declaration
            IsStatic = context.STATIC() != null;
            TypeName = context.qualifiedName().GetText();
            IsImportOnDemand = context.MUL() != null;
        }

        public bool IsStatic { get; }
        public bool IsImportOnDemand { get; }
        public string TypeName { get; }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return context.GetText();
        }
    }
}
