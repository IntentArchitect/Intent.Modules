using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaInterfaceMergeTests
    {
        [Fact]
        public void AddsNewInterface()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneInterface, TwoInterfaces);
            Assert.Equal(TwoInterfaces, result);
        }

        [Fact]
        public void OverwritesExistingInterface()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneInterface, OneInterfaceChanged);
            Assert.Equal(OneInterfaceChanged, result);
        }

        [Fact]
        public void SkipsIgnoredInterface()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneInterfaceIgnored, OneInterfaceChanged);
            Assert.Equal(OneInterfaceIgnored, result);
        }

        [Fact]
        public void RemovesOldInterface()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(TwoInterfaces, OneInterfaceChanged);
            Assert.Equal(OneInterfaceChanged, result);
        }

        [Fact]
        public void RemovesOldAndUpdatesExisting()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ThreeInterfaces, TwoInterfaces);
            Assert.Equal(TwoInterfaces, result);
        }

        [Fact]
        public void AddsTwoNewInterfaceInOrder()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneInterface, ThreeInterfaces);
            Assert.Equal(ThreeInterfaces, result);
        }

        public static string OneInterface = @"
public interface OneInterface {
}";

        public static string OneInterfaceChanged = @"
public interface OneInterface {
    void method();
}";


        public static string OneInterfaceIgnored = @"
@IntentIgnore
public interface OneInterface {
    // Custom implementation
}";

        public static string TwoInterfaces = @"
public interface OneInterface {
}

interface TwoInterface {
}";

        public static string ThreeInterfaces = @"
public interface OneInterface {
    // Interface one of three
}

interface ThreeInterface {
    // Interface three of three
}

interface TwoInterface {
    // Interface two of three
}";

    }
}