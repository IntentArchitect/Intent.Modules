using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaInheritanceAndInterfacesTests
    {
        [Fact]
        public void AddsNewInterfaces()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ClassWithoutInterfaces, ClassWithTwoInterfaces);
            Assert.Equal(ClassWithTwoInterfaces, result);
        }

        [Fact]
        public void AddsNewInterfacesToClassWithBase()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ClassWithInheritanceAndWithoutInterfaces, ClassWithTwoInterfaces);
            Assert.Equal(@"
public class TestClass extends BaseClass implements IInterfaceOne, IInterfaceTwo {
    @IntentIgnore
    public TestClass() { // ensure does not overwrite
    }
}", result);
        }

        public static string ClassWithoutInterfaces = @"
public class TestClass {
    @IntentIgnore
    public TestClass() { // ensure does not overwrite
    }
}";

        public static string ClassWithInheritanceAndWithoutInterfaces = @"
public class TestClass extends BaseClass {
    @IntentIgnore
    public TestClass() { // ensure does not overwrite
    }
}";

        public static string ClassWithTwoInterfaces = @"
public class TestClass implements IInterfaceOne, IInterfaceTwo {
    @IntentIgnore
    public TestClass() { // ensure does not overwrite
    }
}";

    }
}