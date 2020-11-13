using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
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
            var javaLexer = new JavaLexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);

            return new JavaFile(tokens);
        }

        private static JavaParser.CompilationUnitContext ParseFile(ITokenStream tokens)
        {
            var parser = new JavaParser(tokens);
            // Parsing succeeds but is wrong (e.g. doesn't pick up imports properly) when this is enabled.
            //parser.Interpreter.PredictionMode = PredictionMode.SLL; // Performance enhancement
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

        public JavaPackage Package { get; set; }
        public IList<JavaImport> Imports => Children.Where(x => x is JavaImport).Cast<JavaImport>().ToList();
        public IReadOnlyList<JavaClass> Classes => Children.Where(x => x is JavaClass).Cast<JavaClass>().ToList();

        public void Replace(JavaNode node, string text)
        {
            Replace(GetPreviousWsToken(node.StartToken), node.StopToken, text);
        }

        public void Replace(IToken start, IToken stop, string text)
        {
            _rewriter.Replace(start, stop, text);
            UpdateContext();
        }

        public void Replace(int start, int stop, string text)
        {
            _rewriter.Replace(start, stop, text);
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
            _rewriter.InsertBefore(GetPreviousWsToken(node.StartToken), text);
            UpdateContext();
        }

        public void InsertBefore(IToken token, string text)
        {
            _rewriter.InsertBefore(token, text);
            UpdateContext();
        }


        public void UpdateContext()
        {
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(GetSource())));
            var javaLexer = new JavaLexer(inputStream);
            _tokens = new CommonTokenStream(javaLexer);
            _rewriter = new TokenStreamRewriter(_tokens);
            UpdateContext(ParseFile(_tokens));
            var listener = new JavaFileFactoryListener(this);
            ParseTreeWalker.Default.Walk(listener, Context);
        }

        public (string Text, IToken StartToken) GetCommentsAndWhitespaceBefore(IToken token)
        {
            if (token == null)
            {
                return (null, null);
            }
            if (token.TokenIndex - 1 < 0)
            {
                return ("", null);
            }
            var text = "";
            var previous = _tokens.Get(token.TokenIndex - 1);
            while (previous.Type == JavaLexer.WS || previous.Type == JavaLexer.COMMENT)
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

        public IToken GetPreviousWsToken(IToken token)
        {
            if (token.TokenIndex - 1 < 0)
            {
                return token;
            }

            var previous = _tokens.Get(token.TokenIndex - 1);
            IToken ws = null;
            while (previous.Type == JavaLexer.WS || previous.Type == JavaLexer.COMMENT)
            {
                ws = previous;
                if (previous.TokenIndex - 1 < 0)
                {
                    return previous;
                }
                previous = _tokens.Get(previous.TokenIndex - 1);
            }

            return ws ?? token;
        }

        public IToken GetPreviousToken(IToken token)
        {
            return _tokens.Get(token.TokenIndex - 1);
        }

        public IToken GetNextToken(IToken token)
        {
            return _tokens.Get(token.TokenIndex + 1);
        }

        public object GetCommentsBefore(IToken token)
        {
            var commentToken = _tokens.Get(token.TokenIndex - 1);
            if (commentToken.Type == JavaLexer.COMMENT)
            {
                return commentToken;
            }
            return null;
        }

        public bool ImportExists(JavaImport import)
        {
            return Imports.Any(x => x.Equals(import));
        }

        public bool ImportExists(string import)
        {
            return Imports.Any(x => x.GetTextWithComments() == import.Trim());
        }

        public void AddImport(JavaImport import)
        {
            AddImport(import.GetTextWithComments());
            //Imports.Add(import); // commented out while AST doesn't update after each change to it
        }

        public void AddImport(string import)
        {
            if (Imports.Any())
            {
                InsertAfter(Imports.Last(), import);
            }
            else if (Package != null)
            {
                InsertAfter(Package, Environment.NewLine + import + Environment.NewLine);
            }
            else
            {
                InsertBefore(Children.First(), import + Environment.NewLine + Environment.NewLine);
            }
            //Imports.Add(import); // commented out while AST doesn't update after each change to it
        }

        //public override bool IsMerged()
        //{
        //    return true;
        //}
        public void SetPackage(JavaPackage package)
        {
            if (Package != null)
            {
                Replace(Package, package.GetTextWithComments());
            }
            else
            {
                InsertBefore(Children.First(), package.GetTextWithComments() + Environment.NewLine + Environment.NewLine);
            }
        }
    }
}
