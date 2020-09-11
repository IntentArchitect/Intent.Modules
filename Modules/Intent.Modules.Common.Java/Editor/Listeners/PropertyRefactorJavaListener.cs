using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaParserLib.Listeners
{
    public class PropertyRefactorJavaListener : Java9BaseListener
    {
        private readonly TokenStreamRewriter _rewriter;

        public PropertyRefactorJavaListener(CommonTokenStream commonTokenStream)
        {
            _rewriter = new TokenStreamRewriter(commonTokenStream);
        }

        private readonly List<PropertyRefactor> _propertyRefactors = new List<PropertyRefactor>();

        public override void ExitClassBody([NotNull] Java9Parser.ClassBodyContext context)
        {
            foreach (var propRef in _propertyRefactors)
            {
                _rewriter.InsertBefore(context.Stop,
$@"    public void set{propRef.MemberNamePascal}({propRef.MemberType} {propRef.MemberName}) {{
        this.{propRef.MemberName} = {propRef.MemberName};
    }}
    public {propRef.MemberType} get{propRef.MemberNamePascal}() {{
        return {propRef.MemberName};
    }}
");
            }

            if (_propertyRefactors.Any())
            {
                _rewriter.InsertBefore(context.Stop, Environment.NewLine);
            }
        }

        public override void EnterAnnotation([NotNull] Java9Parser.AnnotationContext context)
        {
            if (context.children.Any(p => p.GetText() == "property"))
            {
                _propertyRefactors.Add(new PropertyRefactor());
                _rewriter.Delete(context.Start, context.Stop);
            }
        }

        /*public override void EnterMemberDeclaration([NotNull] Java9Parser.MemberDeclarationContext context)
        {
            var propRef = _propertyRefactors.FirstOrDefault(p => p.IsUnassigned());
            if (propRef != null)
            {
                propRef.Assign(context.GetChild(0).GetText(), context.GetChild(1).GetText().Replace(";", string.Empty));
            }
        }*/

        public string GetManipulatedCode()
        {
            return _rewriter.GetText();
        }

        private class PropertyRefactor
        {
            private string _memberType;
            private string _memberName;

            public bool IsUnassigned()
            {
                return _memberType == null;
            }

            public void Assign(string memberType, string memberName)
            {
                _memberType = memberType;
                _memberName = memberName;
            }

            public string MemberName => _memberName;
            public string MemberNamePascal => FirstLetterUpper(_memberName);
            public string MemberType => _memberType;

            private string FirstLetterUpper(string text)
            {
                if (string.IsNullOrEmpty(text))
                {
                    return text;
                }

                return Char.ToUpper(text[0]) + text.Substring(1);
            }
        }
    }
}
