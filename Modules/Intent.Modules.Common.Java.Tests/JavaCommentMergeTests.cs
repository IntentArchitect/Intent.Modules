using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaCommentMergeTests
    {
        [Fact]
        public void AddsCommentsAndRemovesWhitespace()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(MethodWithoutComment, MethodWithComment);
            Assert.Equal(MethodWithComment, result);
        }

        [Fact]
        public void RemovesCommentsAndAddsWhitespace()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(MethodWithComment, MethodWithoutComment);
            Assert.Equal(MethodWithoutComment, result);
        }

        [Fact]
        public void OverwritesCommentsOnMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(MethodWithComment, MethodWithDifferentComment);
            Assert.Equal(MethodWithDifferentComment, result);
        }

        public string MethodWithoutComment = @"
@IntentMerge
public class TestClass {


    public string testMethod(string s) {
        // custom implementation - string overload
    }
}";

        public string MethodWithComment = @"
@IntentMerge
public class TestClass {
    /**
    * The HelloWorld program implements an application that
    * simply displays ""Hello World!"" to the standard output.
    *
    * @author  Zara Ali
    * @version 1.0
    * @since   2014-03-31 
    */
    public string testMethod(string s) {
        // custom implementation - string overload
    }
}";

        public string MethodWithDifferentComment = @"
@IntentMerge
public class TestClass {
    /**
    * The HelloWorld program implements an application that does nothing...
    */
    public string testMethod(string s) {
        // custom implementation - string overload
    }
}";
    }
}