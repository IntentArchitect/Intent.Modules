using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace Intent.Modules.Common.Java.Editor.Parser
{
    public static class JavaASTParser
    {
        public static JavaFile Parse(string source)
        {
            //var inputStream = new AntlrInputStream(new MemoryStream(Encoding.UTF8.GetBytes(source)));
            //var javaLexer = new Java9Lexer(inputStream);
            //var tokens = new CommonTokenStream(javaLexer);
            var javaFile = new JavaFile(source);
            return javaFile;
        }
    }
}