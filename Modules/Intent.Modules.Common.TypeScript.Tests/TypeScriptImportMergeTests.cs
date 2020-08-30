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
            Assert.Equal(TwoImport, result);
        }

        [Fact]
        public void AddTypeToExistingImport()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: OneImport, outputContent: TwoImportCombined);
            Assert.Equal(TwoImportCombined, result);
        }


        public static string OneImport = @"
import { NgModule } from '@angular/core';

export class EmptyClass { 
}";
        public static string TwoImport = @"
import { NgModule } from '@angular/core';
import { Component } from '@angular/core';

export class EmptyClass { 
}";

        public static string TwoImportCombined = @"
import { NgModule, Component } from '@angular/core';

export class EmptyClass { 
}";
    }
}
