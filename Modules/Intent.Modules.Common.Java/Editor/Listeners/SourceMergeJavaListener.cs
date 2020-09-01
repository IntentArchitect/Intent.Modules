using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using JavaParserLib.Listeners.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JavaParserLib.Listeners
{
    public class SourceMergeJavaListener : JavaBaseListener
    {
        private readonly TokenStreamRewriter _rewriter;
        private readonly IDictionary<MethodSignatureKey, JavaParser.MethodBodyContext> _ignoredMethods;

        public SourceMergeJavaListener(CommonTokenStream tokens, IDictionary<MethodSignatureKey, JavaParser.MethodBodyContext> ignoredMethods)
        {
            _rewriter = new TokenStreamRewriter(tokens);
            _ignoredMethods = ignoredMethods;
        }

        public override void ExitMethodBody([NotNull] JavaParser.MethodBodyContext context)
        {
            var returnTypeStr = context.Parent.Parent.GetChild(0).GetText();
            var name = context.Parent.Parent.GetChild(1).GetText();
            var paramList = ((ParserRuleContext)context.Parent.Parent.GetChild(2).GetChild(0))
                .children
                .Where(p => p.ChildCount > 0)
                .Select(s => string.Join(" ", ((ParserRuleContext)s).children.Select(t => t.GetText())))
                .ToArray();
            var key = new MethodSignatureKey(returnTypeStr, name, paramList);
            if (_ignoredMethods.ContainsKey(key))
            {
                var body = _ignoredMethods[key];
                _rewriter.Replace(context.Start, context.Stop, body.GetFullText());
            }
        }

        public string GetManipulatedCode()
        {
            return _rewriter.GetText();
        }
    }
}
