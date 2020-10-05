using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

public static class Helpers
{
    public static string GetFullText(this ParserRuleContext context)
    {
        if (context.Start == null || context.Stop == null || context.Start.StartIndex < 0 || context.Stop.StopIndex < 0)
            return context.GetText(); // Fallback

        return context.Start.InputStream.GetText(Interval.Of(context.Start.StartIndex, context.Stop.StopIndex));
    }
}