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


        [Fact]
        public void OrderingOfLargeNumberOfNewImports()
        {
            var merger = new TypeScriptWeavingMerger();
            var result = merger.Merge(existingContent: @"
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }", outputContent: @"
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CollapseModule } from 'ngx-bootstrap/collapse';

import { AppComponent } from './app.component';
import { IntentIgnore, IntentMerge, IntentManage } from './intent/intent.decorators';
import { HeaderComponent } from './shared/header/header.component';
import { FooterComponent } from './shared/footer/footer.component';
import { CoreModule } from './core/core.module';
import { AppRoutingModule } from './app-routing.module';

@IntentMerge()
@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CoreModule,
    AppRoutingModule,
    CollapseModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }");
            Assert.Equal(@"
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { IntentIgnore, IntentMerge, IntentManage } from './intent/intent.decorators';
import { HeaderComponent } from './shared/header/header.component';
import { FooterComponent } from './shared/footer/footer.component';
import { CoreModule } from './core/core.module';
import { AppRoutingModule } from './app-routing.module';

@IntentMerge()
@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    CoreModule,
    AppRoutingModule,
    CollapseModule.forRoot()
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }", result);
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
