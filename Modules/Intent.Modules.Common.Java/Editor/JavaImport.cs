using System;
using System.Collections.Generic;
using System.Text;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaImport
    {
        private readonly Java9Parser.ImportDeclarationContext _context;

        public JavaImport(Java9Parser.ImportDeclarationContext context)
        {
            _context = context;

            // Either this list of checks or you get the first child and consult
            // the RuleIndex and based on that perform a switch statement on the constants
            // found in Java9Parser.RULE_* (matches the names below)
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
    }
}
