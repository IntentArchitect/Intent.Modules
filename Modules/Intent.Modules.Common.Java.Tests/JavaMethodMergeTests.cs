using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaMethodMergeTests
    {
        [Fact]
        public void AddsNewMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethod, TwoMethods);
            Assert.Equal(TwoMethods, result);
        }

        [Fact]
        public void OverwritesExistingMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethodVoid, OneMethod);
            Assert.Equal(OneMethod, result);
        }

        [Fact]
        public void SkipsIgnoredMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethodIgnored, OneMethod);
            Assert.Equal(OneMethodIgnored, result);
        }

        [Fact]
        public void RemovesOldMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(TwoMethods, OneMethod);
            Assert.Equal(OneMethod, result);
        }


        [Fact]
        public void AddsOverloadAtCorrectPlace()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethodIgnored, TwoOverloads);
            Assert.Equal(@"
public class TestClass {
    public string testMethod(string s) {
        // custom implementation - string overload
    }
    @IntentIgnore
    public string testMethod() {
        // custom implementation
    }
}", result);
        }

        //[Fact]
        //public void RemovesOldAndUpdatesExisting()
        //{
        //    var merger = new JavaWeavingMerger();
        //    var result = merger.Merge(ThreeMethodes, TwoMethodes);
        //    Assert.Equal(TwoClasses, result);
        //}

        public static string OneMethodVoid = @"
public class TestClass {
    public void testMethod() {
        // implementation void
    }
}";

        public static string OneMethod = @"
public class TestClass {
    public string testMethod() {
        // implementation returns string
    }
}";

        public static string TwoMethods = @"
public class TestClass {
    public string stringMethod() {
        // implementation
    }

    public int testMethod() {
        // implementation
    }
}";


        public static string OneMethodIgnored = @"
public class TestClass {
    @IntentIgnore
    public string testMethod() {
        // custom implementation
    }
}";

        public static string TwoOverloads = @"
public class TestClass {
    public string testMethod(string s) {
        // custom implementation - string overload
    }

    public string testMethod() {
        // implementation returns string
    }
}";
    }
}
