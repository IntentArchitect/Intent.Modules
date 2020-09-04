using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaFieldAndMethodCombiningMergeTests
    {
        [Fact]
        public void AddsFields()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OnlyMethods, CombinationOfFieldsAndMethods);
            Assert.Equal(@"
public class TestClass {
    private int oneField = 5;

    public bool twoField = false;

    public string stringMethod() {
        // implementation
    }

    @IntentIgnore
    public int testMethod() {
        // implementation - custom
    }
}", result);
        }

        [Fact]
        public void InsertsMembersAtCorrectPlace()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(CombinationOfFieldsAndMethods, ExtendedCombinationOfFieldsMethodsAndConstructors);
            Assert.Equal(ExtendedCombinationOfFieldsMethodsAndConstructors, result);
        }

        public static string OnlyFields = @"public class TestClass {
    private int oneField = 5;

    @IntentIgnore
    public bool twoField = true;

}";

        public static string OnlyMethods = @"
public class TestClass {

    public string stringMethod() {
        // implementation
    }

    @IntentIgnore
    public int testMethod() {
        // implementation - custom
    }
}";

        public static string CombinationOfFieldsAndMethods = @"
public class TestClass {
    private int oneField = 5;

    public bool twoField = false;

    public string stringMethod() {
        // implementation
    }

    @IntentIgnore
    public int testMethod() {
        // implementation
    }
}";

        public static string ExtendedCombinationOfFieldsMethodsAndConstructors = @"
public class TestClass {
    private int oneField = 5;
    public String inBetweenOne = ""In Between One"";

    public bool twoField = false;
    public String inBetweenTwo = ""In Between Two"";

    public TestClass() {
    }

    public string inBetweenStringMethodOne() {
        // implementation in between one
    }

    public string stringMethod() {
        // implementation
    }

    public string inBetweenStringMethodTwo() {
        // implementation in between Two
    }

    @IntentIgnore
    public int testMethod() {
        // implementation
    }
}";

    }
}