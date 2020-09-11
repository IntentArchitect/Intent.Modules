using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaInterfaceMethodMergeTests
    {
        [Fact]
        public void AddsNewMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethodOnMerged, TwoMethods);
            Assert.Equal(@"
@IntentMerge
public interface TestInterface {
    public string stringMethod();
    public int testMethod();
}", result);
        }

        [Fact]
        public void OverwritesExistingMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethodVoid, OneMethodOnMerged);
            Assert.Equal(OneMethodOnMerged, result);
        }

        [Fact]
        public void SkipsIgnoredMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethodIgnored, OneMethodOnMerged);
            Assert.Equal($@"
@IntentMerge{OneMethodIgnored}", result);
        }

        [Fact]
        public void RemovesOldMethod()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(TwoMethods, OneMethodOnMerged);
            Assert.Equal(OneMethodOnMerged, result);
        }


        [Fact]
        public void AddsOverloadAtCorrectPlace()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneMethodIgnored, TwoOverloads);
            Assert.Equal(@"
@IntentMerge
public interface TestInterface {
    public string testMethod(string s);
    @IntentIgnore
    public string testMethod();
}", result);
        }

        [Fact]
        public void SkipsRemovingUnknownMethodsWhenInterfaceMerged()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(TwoMethodsOnMergedInterface, OneMethodOnMerged);
            Assert.Equal(@"
@IntentMerge
public interface TestInterface {
    public string testMethod(string s);
    public string testMethod();
}", result);
        }

        public static string OneMethodVoid = @"
@IntentMerge
public interface TestInterface {
    public void testMethod();
}";

        public static string OneMethodOnMerged = @"
@IntentMerge
public interface TestInterface {
    public string testMethod();
}";

        public static string TwoMethods = @"
public interface TestInterface {
    public string stringMethod();
    public int testMethod();
}";


        public static string OneMethodIgnored = @"
public interface TestInterface {
    @IntentIgnore
    public string testMethod();
}";

        public static string TwoOverloads = @"
@IntentMerge
public interface TestInterface {
    public string testMethod(string s);
    public string testMethod();
}";

        public static string TwoMethodsOnMergedInterface = @"
@IntentMerge
public interface TestInterface {
    public string testMethod(string s);
    public string testMethod();
}";
    }
}