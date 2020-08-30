using System;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptMergerTests
    {
        [Fact]
        public void Merges()
        {

        }

        public static string OutputFile = @"
import { NgModule } from '@angular/core';

export class AppRoutingModule { 
    public doSomething() {
        // does something
    }
}";

        public static string ExistingFile = @"
export class AppRoutingModule { 
    public shouldBeRemoved() {
        // implementation
    }
}";

        public static string MergedResult = @"
import { NgModule } from '@angular/core';

export class AppRoutingModule { 
    public doSomething() {
        // does something
    }
}";
    }
}
