using System;
using Antlr4.Runtime;

namespace Intent.Modules.Common.React.Editor
{
    public class JavaScriptFileFactoryListener : JavaScriptParserBaseListener
    {
        public override void EnterReturnStatement(JavaScriptParser.ReturnStatementContext context)
        {
            base.EnterReturnStatement(context);
            context.PrintTokens();
            context.PrintTree();
            var s = context.GetFullText();
        }

        public override void EnterHtmlElements(JavaScriptParser.HtmlElementsContext context)
        {
            base.EnterHtmlElements(context);
            var s = context.GetFullText();
        }

        public override void EnterHtmlElementExpression(JavaScriptParser.HtmlElementExpressionContext context)
        {
            base.EnterHtmlElementExpression(context);

            var s = context.GetFullText();
        }
    }

    public static class PrinterExtension
    {
        public static void PrintTree(this RuleContext ruleContext, int indention = 0)
        {
            Console.WriteLine($"{new string('\t', indention)}{ruleContext.GetText()} \t\t [{ruleContext.GetType().Name}]{((ruleContext as ParserRuleContext)?.exception != null ? $"(ERROR: {(ruleContext as ParserRuleContext)?.exception.Message})": "")}");

            var parsedRC = ruleContext as ParserRuleContext;
            if (parsedRC?.children == null || parsedRC.children.Count == 0)
            {
                return;
            }
            foreach (var ruleContextChild in parsedRC.children)
            {
                (ruleContextChild as ParserRuleContext)?.PrintTree(indention + 1);
            }
        }

        public static void PrintTokens(this ParserRuleContext ruleContext)
        {
            var current = ruleContext.Start;
            //while (current.TokenIndex < ruleContext.Stop.TokenIndex)
            //{
            //}
        }
    }
}