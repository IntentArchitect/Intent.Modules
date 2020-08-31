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
            var result = merger.Merge(existingContent: EmptyClass, outputContent: OneDecorator);
            Assert.Equal(OneDecorator, result);
        }

        [Fact]
        public void AddsDecoratorToEmptyClassMerged()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: EmptyClassMerged, outputContent: OneDecorator);
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
            var result = merger.Merge(existingContent: OneDecorator, outputContent: TwoDecorators);
            Assert.Equal(@"
@TwoDecorator('two of two')
@OneDecorator('one of two')
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
            var result = merger.Merge(existingContent: TwoDifferentDecoratorsOnMergedClass, outputContent: TwoDecorators);
            Assert.Equal(@"
@TwoDecorator('two of two')
@OneDecorator('one of two')
@OneDifferentDecorator('one of two')
@TwoDifferentDecorator('two of two')
@IntentMerge
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}", result);
        }


        public static string EmptyClass = @"
export class EmptyClass {
}";

        public static string EmptyClassMerged = @"
@IntentMerge()
export class EmptyClass {
}";

        public static string OneDecorator = @"
@OneDecorator('one of one')
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}";

        public static string TwoDecorators = @"
@OneDecorator('one of two')
@TwoDecorator('two of two')
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}";

        public static string TwoDifferentDecoratorsOnMergedClass = @"
@OneDifferentDecorator('one of two')
@TwoDifferentDecorator('two of two')
@IntentMerge
export class EmptyClass {
    @IntentIgnore()
    someMethod() { // prevent straight override
    }
}";
    }
}
