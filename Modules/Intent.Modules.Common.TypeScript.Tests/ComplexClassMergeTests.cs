using Intent.Modules.Common.TypeScript.Weaving;
using Xunit;

namespace Intent.Modules.Common.TypeScript.Tests
{
    public class ComplexClassMergeTests
    {
        [Fact]
        public void Test1()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"import { Component, NgModule } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
@NewDecorator()
export class AppComponent {
  title = 'AngularApp';

  constructor() {
      this.title = ""Other"";
    }


    @IntentIgnore()
    customMethod() {
    }
}", outputContent: @"import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'AngularApp';

  addAMethod() {
  }
}");
            Assert.Equal(@"import { Component, NgModule } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'AngularApp';


    @IntentIgnore()
    customMethod() {
    }

  addAMethod() {
  }
}", result);
            ;
        }

        [Fact]
        public void Test2()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"import { Component, NgModule } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'AngularApp';

  constructor() {
      this.title = ""Other"";
    }


    customMethod() {
    }
}", outputContent: @"import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'AngularApp';

  addAMethod() {
  }
}");
            Assert.Equal(@"import { Component, NgModule } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styles: []
})
export class AppComponent {
  title = 'AngularApp';

  addAMethod() {
  }
}", result);
        }
    }
}