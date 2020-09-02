using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.Java.Editor.Parser;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaFile
    {
        private readonly string _originalSource;
        private readonly CommonTokenStream _tokens;
        private readonly TokenStreamRewriter _rewriter;

        public JavaFile(string originalSource)
        {
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(originalSource)));
            var javaLexer = new Java9Lexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);
            _originalSource = originalSource;
            _tokens = tokens;
            _rewriter = new TokenStreamRewriter(tokens);
            ParseFile(tokens);
        }

        //public JavaFile(string source, CommonTokenStream tokens)
        //{
        //    _source = source;
        //    _tokens = tokens;
        //    _rewriter = new TokenStreamRewriter(tokens);
        //    ParseFile(tokens);
        //}

        private void ParseFile(CommonTokenStream tokens)
        {
            var parser = new Java9Parser(tokens);
            var listener = new JavaFileFactoryListener(this);
            ParseTreeWalker.Default.Walk(listener, parser.compilationUnit());
        }

        public IList<JavaClass> Classes { get; } = new List<JavaClass>();
        public IList<JavaImport> Imports { get; } = new List<JavaImport>();

        public void Replace(ParserRuleContext context, string text)
        {
            _rewriter.Replace(GetWhitespaceBefore(context) ?? context.Start, context.Stop, text);
        }

        public string GetSource()
        {
            return _rewriter.GetText();
        }

        public override string ToString()
        {
            return GetSource();
        }

        public void InsertAfter(JavaNode node, string text)
        {
            _rewriter.InsertAfter(node.Context.Stop, text);
        }

        public IToken GetWhitespaceBefore(ParserRuleContext context)
        {
            var wsToken = _tokens.Get(context.Start.TokenIndex - 1);
            if (wsToken.Type == Java9Lexer.WS)
            {
                return wsToken;
            }
            return null;

            //var ws = "";
            //var pos = context.Start.StartIndex - 1;
            //var source = GetSource();
            //while (pos >= 0 && char.IsWhiteSpace(source[pos]))
            //{
            //    ws = $"{source[pos]}{ws}";
            //    pos--;
            //}

            //return ws;
        }
    }
}
