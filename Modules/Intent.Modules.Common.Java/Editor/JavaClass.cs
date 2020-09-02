using System;
using System.Collections.Generic;
using System.Linq;
using JavaParserLib;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaClass : JavaNode
    {
        private readonly Java9Parser.ClassDeclarationContext _context;

        public JavaClass(Java9Parser.ClassDeclarationContext context, JavaFile file) : base(context, file)
        {
            _context = context;
            Identifier = _context.normalClassDeclaration().identifier().GetText();
        }

        public override string Identifier { get; }

        //public override string GetText()
        //{
        //    return $"{Environment.NewLine}{Environment.NewLine}{base.GetText()}";
        //}

        //public override string GetText()
        //{
        //    var ws = File.GetWhitespaceBefore(Context);
        //    return $"{ws}{base.GetText()}";
        //}

        public string Name => Identifier;

        public IReadOnlyList<JavaField> Fields => Children.Where(x => x is JavaField).Cast<JavaField>().ToList();
        public IReadOnlyList<JavaMethod> Methods => Children.Where(x => x is JavaMethod).Cast<JavaMethod>().ToList();
        public IReadOnlyList<JavaConstructor> Constructors => Children.Where(x => x is JavaConstructor).Cast<JavaConstructor>().ToList();

        public override bool IsIgnored()
        {
            var annotation = _context.normalClassDeclaration()?.classModifier(0)?.annotation();
            return annotation?.GetText().StartsWith("@IntentIgnore") ?? false;
        }
    }
}