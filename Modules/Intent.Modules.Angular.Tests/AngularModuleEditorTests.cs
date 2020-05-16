using System;
using System.Linq;
using Intent.Modules.Angular;
using Intent.Modules.Angular.Editor;
using Intent.Modules.Common.TypeScript.Editor;
using Xunit;
using Zu.TypeScript;
using Zu.TypeScript.Change;
using Zu.TypeScript.TsTypes;

namespace Intent.Modules.Common.Tests
{
    public class AngularModuleEditorTests
    {
        [Fact]
        public void FindsCodeWithinAnnotation()
        {
            var editor = new TypeScriptFile(Source.AngularModule);
            editor.AddImportIfNotExists("NewComponent", "./new/new.component");

            var x = editor.GetSource();
        }
    }

    public static class Source
    {
        public static string AngularComponent => @"
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home-screen',
  templateUrl: './home-screen.component.html',
  styleUrls: ['./home-screen.component.css']
})
export class HomeScreenComponent implements OnInit {

  constructor() { }

  ngOnInit() {
  }

}
";

        public static string AngularModule => @"
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppComponent } from './app.component';
import { GeneralModule } from ""./general/general.module"";

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    GeneralModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
";
    }
}