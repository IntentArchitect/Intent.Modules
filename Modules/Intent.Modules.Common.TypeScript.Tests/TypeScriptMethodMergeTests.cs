using System;
using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptMethodMergeTests
    {
        [Fact]
        public void AddsMethodToEmptyClass()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: EmptyMergedClass, outputContent: OneMethod);
            Assert.Equal(@"
@IntentMerge()
export class TestClass {

    methodOne() {
        // Implementation - method one of one
    }
}", result);
        }

        [Fact]
        public void AddsNewMethod()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneMethod, outputContent: TwoMethods);
            Assert.Equal(TwoMethods, result);
        }

        [Fact]
        public void AddsNewMethodAndSkipsIgnored()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneIgnoredMethod, outputContent: TwoMethods);
            Assert.Equal(@"
export class TestClass {

    @IntentIgnore()
    methodOne() {
        // Implementation - method one of one (altered)
    }

    methodTwo() {
        // Implementation - method two of two
    }
}", result);
        }

        [Fact]
        public void RemovesMethod()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoMethodsWithOneIgnored, outputContent: OneMethod);
            Assert.Equal(@"
export class TestClass {

    @IntentIgnore()
    methodOne() {
        // Implementation - method one of three
    }
}", result);
        }

        [Fact]
        public void RemovesMethodAndReplacesOther()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: ThreeMethodsWithOneIgnored, outputContent: TwoMethods);
            Assert.Equal(@"
export class TestClass {

    @IntentIgnore()
    methodOne() {
        // Implementation - method one of three
    }

    methodTwo() {
        // Implementation - method two of two
    }
}", result);
        }

        [Fact]
        public void OverwritesMethodsWithDecorators()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoMethods, outputContent: TwoMethodsWithDecorators);
            Assert.Equal(TwoMethodsWithDecorators, result);
        }

        public static string EmptyMergedClass = @"
@IntentMerge()
export class TestClass {
}";
        public static string OneMethod = @"
export class TestClass {

    methodOne() {
        // Implementation - method one of one
    }
}";

        public static string OneIgnoredMethod = @"
export class TestClass {

    @IntentIgnore()
    methodOne() {
        // Implementation - method one of one (altered)
    }
}";

        public static string TwoMethods = @"
export class TestClass {

    methodOne() {
        // Implementation - method one of two
    }

    methodTwo() {
        // Implementation - method two of two
    }
}";

        public static string TwoMethodsWithOneIgnored = @"
export class TestClass {

    @IntentIgnore()
    methodOne() {
        // Implementation - method one of three
    }

    methodTwo() {
        // Implementation - method two of two
    }
}";

        public static string TwoMethodsWithDecorators = @"
export class TestClass {

    @SomeDecoratorOne([{
        key: ""value""
    }])
    methodOne() {
        // Implementation - method one of two
    }

    @SomeDecoratorTwo([{
        key: ""value""
    }])
    methodTwo() {
        // Implementation - method two of two
    }
}";

        public static string ThreeMethodsWithOneIgnored = @"
export class TestClass {

    @IntentIgnore()
    methodOne() {
        // Implementation - method one of three
    }

    methodTwo() {
        // Implementation - method two of three
    }

    methodThree() {
        // Implementation - method two of three
    }
}";
    }
}
