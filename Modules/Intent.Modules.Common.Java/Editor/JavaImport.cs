using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaImport : JavaNode
    {
        private readonly Java9Parser.ImportDeclarationContext _context;

        public JavaImport(Java9Parser.ImportDeclarationContext context, JavaFile file) : base(context, file)
        {
            _context = context;

            // We can also split this up into different constructors and have different methods
            // overridden in the listener according to each import declaration
            if (context.singleStaticImportDeclaration() != null)
            {
                IsStatic = true;
                Namespace = context.singleStaticImportDeclaration().typeName().GetText();
                TypeName = context.singleStaticImportDeclaration().identifier().GetText();
            }
            else if (context.singleTypeImportDeclaration() != null)
            {
                Namespace = context.singleTypeImportDeclaration().typeName().packageOrTypeName().GetText();
                TypeName = context.singleTypeImportDeclaration().typeName().identifier().GetText();
            }
            else if (context.staticImportOnDemandDeclaration() != null)
            {
                IsStatic = true;
                IsImportOnDemand = true;
                Namespace = context.staticImportOnDemandDeclaration().typeName().packageOrTypeName().GetText();
                TypeName = context.staticImportOnDemandDeclaration().typeName().identifier().GetText();
            }
            else if (context.typeImportOnDemandDeclaration() != null)
            {
                IsImportOnDemand = true;
                Namespace = context.typeImportOnDemandDeclaration().packageOrTypeName().packageOrTypeName().GetText();
                TypeName = context.typeImportOnDemandDeclaration().packageOrTypeName().identifier().GetText();
            }
        }

        public bool IsStatic { get; }
        public bool IsImportOnDemand { get; }
        public string TypeName { get; }
        public string Namespace { get; }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return context.GetText();
        }
    }
}
