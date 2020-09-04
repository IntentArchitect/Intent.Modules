using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaClassMergeTests
    {
        [Fact]
        public void AddsNewClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneClass, TwoClasses);
            Assert.Equal(TwoClasses, result);
        }

        [Fact]
        public void OverwritesExistingClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneClass, OneClassChanged);
            Assert.Equal(OneClassChanged, result);
        }

        [Fact]
        public void SkipsIgnoredClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneClassIgnored, OneClassChanged);
            Assert.Equal(OneClassIgnored, result);
        }

        [Fact]
        public void RemovesOldClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(TwoClasses, OneClassChanged);
            Assert.Equal(OneClassChanged, result);
        }

        [Fact]
        public void RemovesOldAndUpdatesExisting()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ThreeClasses, TwoClasses);
            Assert.Equal(TwoClasses, result);
        }

        [Fact]
        public void AddsTwoNewClassInOrder()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneClass, ThreeClasses);
            Assert.Equal(ThreeClasses, result);
        }

        public static string OneClass = @"
public class OneClass {
}";

        public static string OneClassChanged = @"
public class OneClass {
    public void method() {
    }
}";


        public static string OneClassIgnored = @"
@IntentIgnore
public class OneClass {
    // Custom implementation
}";

        public static string TwoClasses = @"
public class OneClass {
}

class TwoClass {
}";

        public static string ThreeClasses = @"
public class OneClass {
    // Implementation one of three
}

class ThreeClass {
    // Implementation three of three
}

class TwoClass {
    // Implementation two of three
}";

    }
}
