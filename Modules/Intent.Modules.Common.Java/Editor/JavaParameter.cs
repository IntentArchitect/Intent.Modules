using System;
using System.IO;
using Antlr4.Runtime;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaParameter : JavaNode<JavaParser.FormalParameterContext>
    {
        private readonly int _index;

        public JavaParameter(JavaParser.FormalParameterContext context, JavaNode parent, int index) : base(context, parent)
        {
            _index = index;//;
        }

        public override string GetIdentifier(JavaParser.FormalParameterContext context)
        {
            var parameterList = context.Parent as JavaParser.FormalParameterListContext ?? context.Parent.Parent as JavaParser.FormalParameterListContext; 
            var index = parameterList.children.IndexOf(context);
            var type = $"{context.typeType().GetText()}[{index}]";
            return index.ToString();
        }

        public override void Remove()
        {
            if (((JavaMethod)Parent).Children.IndexOf(this) < ((JavaMethod)Parent).Children.Count - 1)
            {
                File.Replace(File.GetPreviousWsToken(StartToken), File.GetNextToken(StopToken), "");
                return;
            }
            if (((JavaMethod)Parent).Children.Count > 1)
            {
                File.Replace(File.GetPreviousToken(File.GetPreviousWsToken(StartToken)), StopToken, "");
                return;
            }
            File.Replace(StartToken, StopToken, "");
        }
    }
}