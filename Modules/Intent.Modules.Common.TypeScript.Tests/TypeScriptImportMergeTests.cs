using System;
using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptImportMergeTests
    {
        [Fact]
        public void AddsNewClass()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneImport, outputContent: TwoImport);
            Assert.Equal(TwoImportCombined, result);
        }

        [Fact]
        public void AddTypeToExistingImport()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneImport, outputContent: TwoImportCombined);
            Assert.Equal(TwoImportCombined, result);
        }

        [Fact]
        public void AddNewAndTypeToExistingImport()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneImport, outputContent: ThreeImports);
            Assert.Equal(@"
import { NgModule, Component } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

@IntentMerge()
export class EmptyClass { 
}", result);
        }

        public static string OneImport = @"
import { NgModule } from '@angular/core';

@IntentMerge()
export class EmptyClass { 
}";

        public static string TwoImport = @"
import { NgModule } from '@angular/core';
import { Component } from '@angular/core';

@IntentMerge()
export class EmptyClass { 
}";

        public static string TwoImportCombined = @"
import { NgModule, Component } from '@angular/core';

@IntentMerge()
export class EmptyClass { 
}";

        public static string ThreeImports = @"
import { NgModule } from '@angular/core';
import { Component } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

@IntentMerge()
export class EmptyClass { 
}";
    }
}
