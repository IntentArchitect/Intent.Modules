using System;
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
            File = this;
            _tokens = tokens;
            _rewriter = new TokenStreamRewriter(tokens);
            var listener = new JavaFileFactoryListener(this);
            ParseTreeWalker.Default.Walk(listener, Context);
        }

        public IReadOnlyList<JavaClass> Classes => Children.Where(x => x is JavaClass).Cast<JavaClass>().ToList();
        public IList<JavaImport> Imports { get; } = new List<JavaImport>();

        public void Replace(JavaNode node, string text)
        {
            _rewriter.Replace(node.StartToken, node.StopToken, text);
            UpdateContext();
        }

        public string GetSource()
        {
            return _rewriter.GetText();
        }

        public override string GetIdentifier(ParserRuleContext context)
        {
            return null;
        }

        public override string ToString()
        {
            return GetSource();
        }

        public void InsertAfter(JavaNode node, string text)
        {
            _rewriter.InsertAfter(node.Context.Stop, text);
            UpdateContext();
        }

        public void InsertAfter(IToken token, string text)
        {
            _rewriter.InsertAfter(token, text);
            UpdateContext();
        }

        public void InsertBefore(JavaNode node, string text)
        {
            _rewriter.InsertBefore(node.StartToken, text);
            UpdateContext();
        }

        public void InsertBefore(IToken token, string text)
        {
            _rewriter.InsertBefore(token, text + Environment.NewLine);
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

        public (string Text, IToken StartToken) GetCommentsAndWhitespaceBefore(IToken token)
        {
            if (token.TokenIndex - 1 < 0)
            {
                return ("", null);
            }
            var text = "";
            var previous = _tokens.Get(token.TokenIndex - 1);
            while (previous.Type == Java9Lexer.WS || previous.Type == Java9Lexer.COMMENT)
            {
                text = $"{previous.Text}{text}";
                if (previous.TokenIndex - 1 < 0)
                {
                    return (text, previous);
                }
                previous = _tokens.Get(previous.TokenIndex - 1);
            }

            return text != "" ? (text, _tokens.Get(previous.TokenIndex + 1)) : (text, null);
        }

        public object GetCommentsBefore(IToken token)
        {
            var commentToken = _tokens.Get(token.TokenIndex - 1);
            if (commentToken.Type == Java9Lexer.COMMENT)
            {
                return commentToken;
            }
            return null;
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

        //public override bool IsMerged()
        //{
        //    return true;
        //}
    }
}
