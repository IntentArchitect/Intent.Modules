using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaConstructorMergeTests
    {
        [Fact]
        public void AddsNewConstructor()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneConstructor, TwoConstructors);
            Assert.Equal(TwoConstructors, result);
        }

        [Fact]
        public void OverwritesExistingConstructor()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneConstructorEmpty, OneConstructor);
            Assert.Equal(OneConstructor, result);
        }

        [Fact]
        public void SkipsIgnoredConstructor()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneConstructorIgnored, OneConstructor);
            Assert.Equal(OneConstructorIgnored, result);
        }

        [Fact]
        public void RemovesOldConstructor()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(TwoConstructors, OneConstructor);
            Assert.Equal(OneConstructor, result);
        }

        //[Fact]
        //public void RemovesOldAndUpdatesExisting()
        //{
        //    var merger = new JavaWeavingMerger();
        //    var result = merger.Merge(ThreeConstructores, TwoConstructores);
        //    Assert.Equal(TwoClasses, result);
        //}

        public static string OneConstructorEmpty = @"
public class TestClass {
    public TestClass() {
        // implementation one of one
    }
}";

        public static string OneConstructor = @"
public class TestClass {
    public TestClass(String param) {
        // implementation with string - one of one
    }
}";

        public static string TwoConstructors = @"
public class TestClass {
    public TestClass() {
        // implementation one of two
    }

    public TestClass(String param) {
        // implementation with string - two of two
    }
}";


        public static string OneConstructorIgnored = @"
public class TestClass {
    @IntentIgnore
    public TestClass(String param) {
        // implementation with string - custom
    }
}";
       
    }
}
