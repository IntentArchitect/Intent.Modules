using System;
using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptClassMergeTests
    {
        [Fact]
        public void AddsNewClass()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneClass, outputContent: TwoClasses);
            Assert.Equal(@"
import { NgModule } from '@angular/core';

export class FirstClass { 
}

export class SecondClass { 
}", result);
        }

        [Fact]
        public void RemovesOldClass()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoClasses, outputContent: OneClass);
            Assert.Equal(@"
import { NgModule } from '@angular/core';

export class FirstClass { 
}", result);
        }

        [Fact]
        public void SkipsIgnoredClasses()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoIgnoredClasses, outputContent: OneClass);
            Assert.Equal(TwoIgnoredClasses, result);
        }

        [Fact]
        public void OverwritesMatchedClasses()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: TwoClasses, outputContent: TwoImplementedClasses);
            Assert.Equal(TwoImplementedClasses, result);
        }

        [Fact]
        public void SameOutputsSameResult()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneImplementedClass, outputContent: OneImplementedClass);
            Assert.Equal(OneImplementedClass, result);
        }

        [Fact]
        public void CreatesClassWhenNoneExists()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"
import { NgModule } from '@angular/core';", outputContent: OneImplementedClass);
            Assert.Equal(OneImplementedClass, result);
        }

        public static string OneClass = @"
import { NgModule } from '@angular/core';

export class FirstClass { 
}";

        public static string OneImplementedClass = @"
import { NgModule } from '@angular/core';

export class FirstClass {
    public propertyOne: string;

    someMethod() {
        var x = 0;
        return x;
    }

    @WithDecorator()
    someOtherMethod(param: string[]) {
        var x = 0;
        return x;
    }
}";

        public static string OneDecoratedClass = @"
import { NgModule } from '@angular/core';

@ClassDecorator()
export class FirstClass {
    // Implementation - first class of one;
}";

        public static string TwoClasses = @"
import { NgModule } from '@angular/core';

export class FirstClass { 
}

export class SecondClass { 
}";

        public static string TwoIgnoredClasses = @"
import { NgModule } from '@angular/core';

@IntentIgnore()
export class FirstClass {
    // special implementations
}

@IntentIgnore()
export class SecondClass { 
    // special implementations
}";

        public static string TwoImplementedClasses = @"
import { NgModule } from '@angular/core';

export class FirstClass {
    // Implementation - first class of two;
}

export class SecondClass {
    // Implementation - second class of two;
}";
    }
}
