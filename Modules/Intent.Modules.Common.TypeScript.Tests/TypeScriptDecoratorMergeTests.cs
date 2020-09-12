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

        [Fact]
        public void MergesInternalsOfDecorator()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

@IntentMerge()
@NgModule({
  imports: [],
  providers: [],
  bootstrap: []
})
@ExtraDecorator('some', 3)
export class AppModule { }", outputContent: ComplexAngularAppDecoration);
            Assert.Equal(ComplexAngularAppDecoration, result);
        }

        [Fact]
        public void AddsTypeToInnerArrayOfDecorator()
        {
            
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UsersRouting } from './users-routing.module';
import { IntentMerge } from ""../../shared/intent.decorators"";
import { UserDetailsComponent } from ""./user-details/user-details.component"";

@IntentMerge
@NgModule({
  declarations: [
    UserDetailsComponent
  ],
  imports: [
    CommonModule,
    UsersRouting
  ]
})
export class UsersModule { }
", outputContent: @"import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserSearchComponent } from './user-search/user-search.component';
import { UsersRouting } from './users-routing.module';
import { IntentMerge } from ""../../shared/intent.decorators"";

@IntentMerge
@NgModule({
  declarations: [
    UserSearchComponent
  ],
  imports: [
    CommonModule,
    UsersRouting
  ]
})
export class UsersModule { }
");
            Assert.Equal(@"import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserSearchComponent } from './user-search/user-search.component';
import { UsersRouting } from './users-routing.module';
import { IntentMerge } from ""../../shared/intent.decorators"";
import { UserDetailsComponent } from ""./user-details/user-details.component"";

@IntentMerge
@NgModule({
  declarations: [
    UserSearchComponent,
    UserDetailsComponent
  ],
  imports: [
    CommonModule,
    UsersRouting
  ]
})
export class UsersModule { }
", result);
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

        public static string ComplexAngularAppDecoration = @"
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { CoreModule } from './core/core.module';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { HeaderComponent } from './shared/header/header.component';
import { FooterComponent } from './shared/footer/footer.component';

@IntentMerge()
@NgModule({
  declarations: [
    HeaderComponent,
    AppComponent,
    FooterComponent
  ],
  imports: [
    BrowserModule, 
    AppRoutingModule, 
    CoreModule,
    CollapseModule.forRoot(),
    BrowserAnimationsModule,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
@ExtraDecorator('some-string', 5)
export class AppModule { }";
    }
}
