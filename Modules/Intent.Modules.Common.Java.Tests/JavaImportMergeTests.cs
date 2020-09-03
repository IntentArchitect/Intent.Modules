using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaImportMergeTests
    {
        [Fact]
        public void AddsNewClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneImport, ThreeImports);
            // order gets jumbled here because the AST doesn't get updated after each change (i.e. add import)
            Assert.Equal(@"
import static java.lang.Math.*;
import org.lib.Class.*;
import java.lang.System;

public class TestClass {
}", result);
        }

        public static string OneImport = @"
import static java.lang.Math.*;

public class TestClass {
}";

        public static string ThreeImports = @"
import static java.lang.Math.*;
import java.lang.System;
import org.lib.Class.*;

public class TestClass {
}";
    }
}