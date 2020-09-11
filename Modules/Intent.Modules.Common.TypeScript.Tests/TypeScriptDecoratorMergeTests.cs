using System;
using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptDecoratorMergeTests
    {
        [Fact]
        public void AddsDecoratorToEmptyClass()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: ClassWithNoDecorators, outputContent: ClassWithOneDecorator);
            Assert.Equal(ClassWithOneDecorator, result);
        }

        [Fact]
        public void AddsDecoratorToEmptyClassMerged()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: EmptyClassMerged, outputContent: ClassWithOneDecorator);
            Assert.Equal(@"
@OneDecorator('one of one')
@IntentMerge()
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}", result);
        }

        [Fact]
        public void UpdatesDecorators()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: ClassWithOneDecorator, outputContent: ClassTwoDecorators);
            Assert.Equal(@"
@OneDecorator('one of two')
@TwoDecorator('two of two')
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}", result);
        }

        [Fact]
        public void SkipsExistingAndAddsDecoratorsOnMergedClass()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: MergedClassWithTwoDifferentDecorators, outputContent: ClassTwoDecorators);
            Assert.Equal(@"
@OneDecorator('one of two')
@OneDifferentDecorator('one of two')
@TwoDecorator('two of two')
@TwoDifferentDecorator('two of two')
@IntentMerge()
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}", result);
        }

        [Fact]
        public void AddsDecoratorToMethodWithNoDecorators()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: MethodWithNoDecorators, outputContent: MergedMethodWithOneDecorator);
            Assert.Equal(MergedMethodWithOneDecorator, result);
        }

        [Fact]
        public void AddsDecoratorToMethodWithOneDecorators()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: MergedMethodWithOneDecorator, outputContent: MergedMethodWithTwoDecorator);
            Assert.Equal(MergedMethodWithTwoDecorator, result);
        }

        [Fact]
        public void SkipsRemovingDecoratorFromMethodWhenMerged()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: MergedMethodWithTwoDecorator, outputContent: MergedMethodWithOneDecorator);
            Assert.Equal(MergedMethodWithTwoDecorator, result);
        }


        public static string ClassWithNoDecorators = @"
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}";

        public static string EmptyClassMerged = @"
@IntentMerge()
export class EmptyClass {
}";

        public static string ClassWithOneDecorator = @"
@OneDecorator('one of one')
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}";

        public static string ClassTwoDecorators = @"
@OneDecorator('one of two')
@TwoDecorator('two of two')
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}";

        public static string MergedClassWithTwoDifferentDecorators = @"
@OneDifferentDecorator('one of two')
@TwoDifferentDecorator('two of two')
@IntentMerge()
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}";

        public static string MethodWithNoDecorators = @"
@IntentMerge
export class EmptyClass {
    someMethod() {
        // some implementation
    }
}";
        public static string MergedMethodWithOneDecorator = @"
@IntentMerge
export class EmptyClass {
    @OneDecorator()
    @IntentMerge()
    someMethod() {
        // some implementation
    }
}";

        public static string MergedMethodWithTwoDecorator = @"
@IntentMerge
export class EmptyClass {
    @OneDecorator()
    @TwoDecorator()
    @IntentMerge()
    someMethod() {
        // some implementation
    }
}";
    }
}
