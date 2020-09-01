using System;
using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptInterfaceMergeTests
    {
        [Fact]
        public void AddsNewInterface()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneInterface, outputContent: TwoInterfaces);
            Assert.Equal(TwoInterfaces, result);
        }

        [Fact]
        public void RemovesOldInterface()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoInterfaces, outputContent: OneInterface);
            Assert.Equal(OneInterface, result);
        }

        [Fact]
        public void SkipsIgnoredInterfaces()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoIgnoredInterfaces, outputContent: OneInterface);
            Assert.Equal(TwoIgnoredInterfaces, result);
        }

        [Fact]
        public void OverwritesMatchedClasses()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoInterfaces, outputContent: TwoInterfacesDifferentImplementations);
            Assert.Equal(TwoInterfacesDifferentImplementations, result);
        }


        public static string OneInterface = @"
export interface OneInterface {
    property: string;
    method(): void;
}";
        public static string TwoInterfaces = @"
export interface OneInterface {
    property: string;
    method(): void;
}

export interface TwoInterface {
    property: string;
    method(): void;
}";

        public static string TwoInterfacesDifferentImplementations = @"
export interface OneInterface {
    propertyOne: string;
    methodOne(): void;
}

export interface TwoInterface {
    propertyTwo: string;
    methodTwo(): void;
}";
        public static string TwoIgnoredInterfaces = @"
//@IntentIgnore()
export interface OneInterface {
    property: string;
    method(): void;
}

//@IntentIgnore()
export interface TwoInterface {
    property: string;
    method(): void;
}";
    }
}
