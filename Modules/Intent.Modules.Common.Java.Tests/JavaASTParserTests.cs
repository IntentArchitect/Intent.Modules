using System;
using System.Linq;
using Intent.Modules.Common.Java.Editor;
using Intent.Modules.Common.Java.Editor.Parser;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaASTParserTests
    {
        [Fact]
        public void ImportsParseCorrectly()
        {
            var javaFile = JavaASTParser.Parse(JavaTestFile);
            Assert.Equal(3, javaFile.Imports.Count);

            var import1 = javaFile.Imports[0];
            Assert.Equal("java.lang", import1.Namespace);
            Assert.Equal("Math", import1.TypeName);
            Assert.True(import1.IsImportOnDemand);
            Assert.True(import1.IsStatic);

            var import2 = javaFile.Imports[1];
            Assert.Equal("java.lang", import2.Namespace);
            Assert.Equal("System", import2.TypeName);
            Assert.False(import2.IsImportOnDemand);
            Assert.False(import2.IsStatic);

            var import3 = javaFile.Imports[2];
            Assert.Equal("org.lib", import3.Namespace);
            Assert.Equal("Class", import3.TypeName);
            Assert.True(import3.IsImportOnDemand);
            Assert.False(import3.IsStatic);
        }

        [Fact]
        public void ClassParsesCorrectly()
        {
            var javaFile = JavaASTParser.Parse(JavaTestFile);
            Assert.Equal(1, javaFile.Classes.Count);
            var @class = javaFile.Classes.Single();
            Assert.Equal("Employee", @class.Identifier);
        }

        [Fact]
        public void FieldsParseCorrectly()
        {
            var javaFile = JavaASTParser.Parse(JavaTestFile);
            var @class = javaFile.Classes.Single();
            Assert.Equal(2, @class.Methods.Count);
            var id = @class.Fields[0];
            var name = @class.Fields[1];
            Assert.Equal("id", id.Identifier);
            Assert.Equal("name", name.Identifier);

        }

        [Fact]
        public void ConstructorsParseCorrectly()
        {
            var javaFile = JavaASTParser.Parse(JavaTestFile);
            var @class = javaFile.Classes.Single();
            Assert.Equal(2, @class.Constructors.Count);
            var constructor1 = @class.Constructors[0];
            var constructor2 = @class.Constructors[1];
            Assert.Equal("", constructor1.Identifier);
            Assert.Equal("int, String", constructor2.Identifier);

        }

        [Fact]
        public void MethodsParseCorrectly()
        {
            var javaFile = JavaASTParser.Parse(JavaTestFile);
            var @class = javaFile.Classes.Single();
            Assert.Equal(2, @class.Fields.Count);
            var somePublicMethod = @class.Methods[0];
            var someDefaultMethod = @class.Methods[1];
            Assert.Equal("somePublicMethod", somePublicMethod.Identifier);
            Assert.Equal("someDefaultMethod", someDefaultMethod.Identifier);
        }

        public static string JavaTestFile = @"
import static java.lang.Math.*; 
import java.lang.System;
import org.lib.Class.*;

public class Employee implements java.io.Serializable {
    @property()
    private int id;
    @property()
    private String name;

    public Employee() {}

    public Employee(int id, String name) {
        this.id = id;
        this.name = name;
    }

	public void somePublicMethod(String param) {
        // void somePublicMethod(String param) implementation
    }

    String someDefaultMethod(int param) {
        // String someDefaultMethod(int param) implementation
    }
}";
    }
}
