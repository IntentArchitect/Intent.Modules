using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using JavaParserLib;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaClass : JavaNode
    {
        private readonly Java9Parser.ClassDeclarationContext _context;

        public JavaClass(Java9Parser.ClassDeclarationContext context, JavaNode parent) : base(context, parent)
        {
            _context = context;
        }

        protected override string GetIdentifier(ParserRuleContext context)
        {
            return ((Java9Parser.ClassDeclarationContext) context).normalClassDeclaration().identifier().GetText();
        }

        public string Name => Identifier;
        public IReadOnlyList<JavaField> Fields => Children.Where(x => x is JavaField).Cast<JavaField>().ToList();
        public IReadOnlyList<JavaMethod> Methods => Children.Where(x => x is JavaMethod).Cast<JavaMethod>().ToList();
        public IReadOnlyList<JavaConstructor> Constructors => Children.Where(x => x is JavaConstructor).Cast<JavaConstructor>().ToList();

        //public override bool IsIgnored()
        //{
        //    var annotation = _context.normalClassDeclaration()?.classModifier(0)?.annotation();
        //    return annotation?.GetText().StartsWith("@IntentIgnore") ?? false;
        //}

        //public override bool IsMerged()
        //{
        //    var annotation = _context.normalClassDeclaration()?.classModifier(0)?.annotation();
        //    return annotation?.GetText().StartsWith("@IntentMerge") ?? false;
        //}
    }
}