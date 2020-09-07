using System;
using Intent.Modules.Common.TypeScript.Editor;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class TypeScriptParsingTests
    {
        [Fact]
        public void FindsImports()
        {
            var file = new TypeScriptFileEditor(TypeScriptFile).File;
            Assert.Equal(2, file.Imports.Count);
            var coreImports = file.Imports[0];
            Assert.Equal("'@angular/core'", coreImports.Location);
            Assert.Collection(coreImports.Types, x => Assert.Equal("Component", x), x => Assert.Equal("NgModule", x));
        }

        [Fact]
        public void FindsClasses()
        {
            var file = new TypeScriptFileEditor(TypeScriptFile).File;
            Assert.Equal(2, file.Classes.Count);
        }

        [Fact]
        public void FindsMethods()
        {
            var file = new TypeScriptFileEditor(TypeScriptFile).File;
            Assert.Collection(file.Classes,
                x =>
                {
                    Assert.Equal(2, x.GetChildren<TypeScriptMethod>().Count);
                }, x =>
                {
                    Assert.Equal(1, x.GetChildren<TypeScriptMethod>().Count);
                });

        }

        public static string TypeScriptFile = @"
import { Component, NgModule } from '@angular/core';
import { Routes, RouterModule, PreloadAllModules } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
@NewDecorator()
export class AppComponent {
  title = 'AngularApp';
  model: ModelDTO;

  constructor() {
    this.title = ""Other"";
  }

  get myProperty(): string { return this.model.property; }
  set myProperty(value: string) { this.model.property = value; }
  
  @IntentIgnore()
  customMethod() {
  }

  otherMethod(s: string) {
    // do some stuff;
  }
}

class SubClass {
  someMethod() {
  }
}";
    }
}