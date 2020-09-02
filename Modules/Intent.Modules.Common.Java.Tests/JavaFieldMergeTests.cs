using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaFieldMergeTests
    {
        [Fact]
        public void AddsNewField()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneField, TwoFields);
            Assert.Equal(TwoFields, result);
        }

        [Fact]
        public void OverwritesExistingField()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneFieldVoid, OneField);
            Assert.Equal(OneField, result);
        }

        [Fact]
        public void SkipsIgnoredField()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneFieldIgnored, OneField);
            Assert.Equal(OneFieldIgnored, result);
        }

        [Fact]
        public void RemovesOldField()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(TwoFields, OneField);
            Assert.Equal(OneField, result);
        }

        //[Fact]
        //public void RemovesOldAndUpdatesExisting()
        //{
        //    var merger = new JavaWeavingMerger();
        //    var result = merger.Merge(ThreeFieldes, TwoFieldes);
        //    Assert.Equal(TwoClasses, result);
        //}

        public static string OneFieldVoid = @"
public class TestClass {
    private int oneField;
}";

        public static string OneField = @"
public class TestClass {
    private int oneField = 5;
}";

        public static string TwoFields = @"
public class TestClass {
    private int oneField = 5;

    public bool twoField = false;

}";


        public static string OneFieldIgnored = @"
public class TestClass {
    @IntentIgnore
    private int oneField = 555;
}";

    }
}
