﻿using System;
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
        }

        [Fact]
        public void ClassParsesCorrectly()
        {
            var javaFile = JavaASTParser.Parse(JavaTestFile);
            Assert.Equal(1, javaFile.Classes.Count);
            var @class = javaFile.Classes.Single();
            Assert.Equal("Employee", @class.Name);
        }

        [Fact]
        public void MethodsParseCorrectly()
        {
            var javaFile = JavaASTParser.Parse(JavaTestFile);
            var @class = javaFile.Classes.Single();
            Assert.Equal(2, @class.Methods.Count);
            var somePublicMethod = @class.Methods[0];
            var someDefaultMethod = @class.Methods[1];
            Assert.Equal("somePublicMethod", somePublicMethod.Name);
            Assert.Equal("someDefaultMethod", someDefaultMethod.Name);
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

	public void somePublicMethod(String param) {
        // void somePublicMethod(String param) implementation
    }

    String someDefaultMethod(int param) {
        // String someDefaultMethod(int param) implementation
    }
}";
    }
}
