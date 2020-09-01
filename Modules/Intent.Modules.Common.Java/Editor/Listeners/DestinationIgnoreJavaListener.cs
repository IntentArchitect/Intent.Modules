using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Antlr4.Runtime;
using JavaParserLib.Listeners.Models;

namespace JavaParserLib.Listeners
{
    public class DestinationIgnoreJavaListener : JavaBaseListener
    {
        private Dictionary<MethodSignatureKey, JavaParser.MethodBodyContext> _ignoredMethods = new Dictionary<MethodSignatureKey, JavaParser.MethodBodyContext>();

        public override void ExitMethodBody([NotNull] JavaParser.MethodBodyContext context)
        {
            var ignoreAnnotate = context.Parent.Parent.Parent.GetChild(0).GetChild(0).GetText();
            if (ignoreAnnotate == "@Ignore")
            {
                var returnTypeStr = context.Parent.Parent.GetChild(0).GetText();
                var name = context.Parent.Parent.GetChild(1).GetText();
                var paramList = ((ParserRuleContext)context.Parent.Parent.GetChild(2).GetChild(0))
                    .children
                    .Where(p => p.ChildCount > 0)
                    .Select(s => string.Join(" ", ((ParserRuleContext)s).children.Select(t => t.GetText())))
                    .ToArray();
                _ignoredMethods.Add(new MethodSignatureKey(returnTypeStr, name, paramList), context);
            }
        }

        public IDictionary<MethodSignatureKey, JavaParser.MethodBodyContext> GetIgnoredMethods()
        {
            return _ignoredMethods;
        }
    }
}
