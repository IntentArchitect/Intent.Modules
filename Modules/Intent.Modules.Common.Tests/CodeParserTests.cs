using Intent.Modules.Common.Templates;
using Xunit;

namespace Intent.Modules.Common.Tests
{
    public class CodeParserTests
    {
        [Fact]
        public void FindsCodeWithinAnnotation()
        {
            var source = @"
@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    GeneralModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
";

            Assert.Equal(@"{
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    GeneralModule
  ],
  providers: [],
  bootstrap: [AppComponent]
}", source.CodeInside(@"@NgModule", '(', ')'));
        }

        [Fact]
        public void RecognizesInnerBrackets()
        {
            var source = @"
export class HomeScreenComponent implements OnInit {

  constructor() { }

  ngOnInit() {
    //My code here

    for(int i = 0; i < 10; i++) {
    }
  }

}
";
            var result = source.CodeInside(@"ngOnInit", '{', '}');
            Assert.Equal(@"
    //My code here

    for(int i = 0; i < 10; i++) {
    }
  ", result);
        }

        [Fact]
        public void FindsNgModuleImports()
        {
            var source = @"
@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    GeneralModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
";
            var result = source.CodeInside(@"NgModule", '(', ')')
                .CodeInside("imports", '[', ']');
            Assert.Equal(@"
    BrowserModule,
    GeneralModule
  ", result);
        }

        [Fact]
        public void ReplacesCode()
        {
            var source = @"
@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    GeneralModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})";
            var result = source.ReplaceCodeInside(@"NgModule", '(', ')',
                source.MethodParameters(@"NgModule")
                    .ReplaceCodeInside("imports", '[', ']', @"
    BrowserModule,
    GeneralModule,
    NewModule
  "));
            Assert.Equal(@"
@NgModule({
  declarations: [AppComponent],
  imports: [
    BrowserModule,
    GeneralModule,
    NewModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})", result);
        }
    }
}