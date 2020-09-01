using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public class JavaASTParser
    {
        public static JavaFile Parse(string source)
        {
            
            var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(source)));
            var javaLexer = new JavaLexer(inputStream);
            var tokens = new CommonTokenStream(javaLexer);
            var parser = new JavaParser(tokens);
            var listener = new JavaFileFactoryListener();
            ParseTreeWalker.Default.Walk(listener, parser.compilationUnit());
            return listener.JavaFile;
        }
    }
}