using System;
using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptConstructorMergeTests
    {
        [Fact]
        public void AddsConstructor()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: WithoutConstructor, outputContent: WithConstructor);
            Assert.Equal(WithConstructor, result);
        }

        [Fact]
        public void AddsConstructorOnMergedClass()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: MergedClassWithoutConstructor, outputContent: WithConstructor);
            Assert.Equal(@"
@IntentMerge()
export class EmptyClass { 
    constructor() {
        // does something
    }

    @IntentIgnore() // ensure constructor is merged
    someMethod() {
    }
}", result);
        }

        [Fact]
        public void OverwritesConstructor()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: WithConstructor, outputContent: WithConstructorWithParameters);
            Assert.Equal(WithConstructorWithParameters, result);
        }

        [Fact]
        public void IgnoresIgnoredConstructor()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: WithIgnoredConstructor, outputContent: WithConstructorWithParameters);
            Assert.Equal(WithIgnoredConstructor, result);
        }

        public static string WithoutConstructor = @"
export class EmptyClass { 
}";

        public static string MergedClassWithoutConstructor = @"
@IntentMerge()
export class EmptyClass {
}";

        public static string WithConstructor = @"
export class EmptyClass { 
    constructor() {
        // does something
    }

    @IntentIgnore() // ensure constructor is merged
    someMethod() {
    }
}";

        public static string WithIgnoredConstructor = @"
export class EmptyClass { 
    
    @IntentIgnore()
    constructor() {
        // does something (customized)
    }

    @IntentIgnore() // ensure constructor is merged
    someMethod() {
    }
}";

        public static string WithConstructorWithParameters = @"
export class EmptyClass { 
    constructor(param1: string, param2: string) {
        // does something with parameters
    }

    @IntentIgnore() // ensure constructor is merged
    someMethod() {
    }
}";
    }
}
