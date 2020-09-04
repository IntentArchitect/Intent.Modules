using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Intent.Modules.Common.Java.Editor.Parser;

namespace Intent.Modules.Common.Java.Editor
{
    public class JavaFile : JavaNode
    {
        private CommonTokenStream _tokens;
        private TokenStreamRewriter _rewriter;

        public static JavaFile Parse(string originalSource)
        {
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(originalSource)));
            var javaLexer = new Java9Lexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);

            return new JavaFile(tokens);
        }

        private static Java9Parser.CompilationUnitContext ParseFile(ITokenStream tokens)
        {
            var parser = new Java9Parser(tokens);
            return parser.compilationUnit();
        }

        public JavaFile(CommonTokenStream tokens) : base(ParseFile(tokens), null)
        {
            _tokens = tokens;
            _rewriter = new TokenStreamRewriter(tokens);
            var listener = new JavaFileFactoryListener(this);
            ParseTreeWalker.Default.Walk(listener, Context);
        }

        public IReadOnlyList<JavaClass> Classes => Children.Where(x => x is JavaClass).Cast<JavaClass>().ToList();
        public IList<JavaImport> Imports { get; } = new List<JavaImport>();

        public void Replace(ParserRuleContext context, string text)
        {
            _rewriter.Replace(GetWhitespaceBefore(context) ?? context.Start, context.Stop, text);
            UpdateContext();
        }

        public string GetSource()
        {
            return _rewriter.GetText();
        }

        protected override string GetIdentifier(ParserRuleContext context)
        {
            return null;
        }

        public override string ToString()
        {
            return GetSource();
        }

        public void InsertAfter(JavaNode after, JavaNode nodeToInsert)
        {
            Children.Insert(Children.IndexOf(after) + 1, nodeToInsert);
            InsertAfter(after, nodeToInsert.GetText());
        }

        public void InsertAfter(JavaNode node, string text)
        {
            _rewriter.InsertAfter(node.Context.Stop, text);
            UpdateContext();
        }

        public void UpdateContext()
        {
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(GetSource())));
            var javaLexer = new Java9Lexer(inputStream);
            _tokens = new CommonTokenStream(javaLexer);
            _rewriter = new TokenStreamRewriter(_tokens);
            UpdateContext(ParseFile(_tokens));
            var listener = new JavaFileFactoryListener(this);
            ParseTreeWalker.Default.Walk(listener, Context);
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

        public bool ImportExists(JavaImport import)
        {
            return Imports.Any(x => x.Equals(import));
        }

        public void AddImport(JavaImport import)
        {
            InsertAfter(Imports.Last(), import.GetText());
            //Imports.Add(import); // commented out while AST doesn't update after each change to it
        }
    }
}
