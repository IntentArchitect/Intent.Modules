using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaImportMergeTests
    {
        [Fact]
        public void AddsNewImports()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(OneImport, ThreeImports);
            Assert.Equal(@"
import static java.lang.Math.*;
import java.lang.System;
import org.lib.Class.*;

public class TestClass {
}", result);
        }

        [Fact]
        public void AddsNewImportsWhereNoneExisted()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(@"
public class TestClass {
}", ThreeImports);
            Assert.Equal(@"
import static java.lang.Math.*;
import java.lang.System;
import org.lib.Class.*;

public class TestClass {
}", result);
        }

        [Fact]
        public void AddsNewImportsAfterPackage()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(@"
package com.test;

public class TestClass {
}", ThreeImports);
            Assert.Equal(@"
package com.test;
import static java.lang.Math.*;
import java.lang.System;
import org.lib.Class.*;

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