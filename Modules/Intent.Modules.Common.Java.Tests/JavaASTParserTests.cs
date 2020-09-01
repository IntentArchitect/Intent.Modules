using System;
using System.Linq;
using Intent.Modules.Common.Java.Editor.Parser;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaASTParserTests
    {
        [Fact]
        public void ParsingSucceeds()
        {
            var javaFile = JavaASTParser.Parse(@"
package mypack;

public class Employee implements java.io.Serializable {
    @property()
    private int id;
    @property()
    private String name;

    public Employee() {}

	public void somePublicMethod(String param) {
        // void somePublicMethod(String param) implementation
    }

    String someDefaultMethod(int param) {
        // String someDefaultMethod(int param) implementation
    }
}");
            Assert.Equal(1, javaFile.Classes.Count);
            var @class = javaFile.Classes.Single();
            Assert.Equal("Employee", @class.Name);
            Assert.Equal(2, @class.Methods.Count);
            var somePublicMethod = @class.Methods[0];
            var someDefaultMethod = @class.Methods[1];
            Assert.Equal("somePublicMethod", somePublicMethod.Name);
            Assert.Equal("someDefaultMethod", someDefaultMethod.Name);
        }
    }
}
