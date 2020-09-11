using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaClassInheritanceAndInterfacesTests
    {
        [Fact]
        public void AddsSuperClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ClassWithoutSuperOrInterfaces, ClassWithInheritanceAndWithoutInterfaces);
            Assert.Equal(ClassWithInheritanceAndWithoutInterfaces, result);
        }

        [Fact]
        public void RemovesSuperClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ClassWithInheritanceAndWithoutInterfaces, ClassWithoutSuperOrInterfaces);
            Assert.Equal(ClassWithoutSuperOrInterfaces, result);
        }

        [Fact]
        public void AddsNewInterfaces()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ClassWithoutSuperOrInterfaces, ClassWithTwoInterfaces);
            Assert.Equal(ClassWithTwoInterfaces, result);
        }

        [Fact]
        public void RemovesAllInterfaces()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ClassWithTwoInterfaces, ClassWithoutSuperOrInterfaces);
            Assert.Equal(ClassWithoutSuperOrInterfaces, result);
        }

        [Fact]
        public void UpdatesInterfaces()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(ClassWithTwoInterfaces, @"
public class TestClass implements IOtherInterfaceOne, IInterfaceOne, IOtherInterfaceTwo {}");
            Assert.Equal(@"
public class TestClass implements IOtherInterfaceOne, IInterfaceOne, IOtherInterfaceTwo {
    @IntentIgnore
    public TestClass() { // ensure does not overwrite
    }
}", result);
        }

        [Fact]
        public void MergeUpdatesInterfacesAdditively()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge($"@IntentMerge{ClassWithTwoInterfaces}", @"
public class TestClass implements IOtherInterfaceOne, IInterfaceOne, IOtherInterfaceTwo {}");
            Assert.Equal(@"@IntentMerge
public class TestClass implements IOtherInterfaceOne, IInterfaceOne, IOtherInterfaceTwo, IInterfaceTwo {
    @IntentIgnore
    public TestClass() { // ensure does not overwrite
    }
}", result);
        }

        [Fact]
        public void AddsNewInterfacesToMergedClassWithBase()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge($"@IntentMerge{ClassWithInheritanceAndWithoutInterfaces}", ClassWithTwoInterfaces);
            Assert.Equal(@"@IntentMerge
public class TestClass extends BaseClass implements IInterfaceOne, IInterfaceTwo {
    @IntentIgnore
    public TestClass() { // ensure does not overwrite
    }
}", result);
        }

        public static string ClassWithoutSuperOrInterfaces = @"
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