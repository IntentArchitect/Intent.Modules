using System;
using System.Collections.Generic;
using System.Text;
using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaClassMethodSignatureMergeTests
    {
        [Fact]
        public void AddsParametersToEmptySignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod() {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String newParam) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String newParam) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void AddsParametersToSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing, Boolean newParam) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing, Boolean newParam) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void InsertsParametersToBeginningOfSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, String existing) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, String existing) {
        // my special implementation
    }
}", result);
        }


        [Fact]
        public void InsertsParametersInMiddleOfSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing, Boolean otherExisting) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing, DataType newParam, Boolean otherExisting) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing, DataType newParam, Boolean otherExisting) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void RemovesParameterFromBeginningSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, String existing) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(String existing) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void RemovesParameterFromEndOfSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, DataType toRemove, String existing) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, String existing) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, String existing) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void RemovesParameterFromMiddleOfSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, String existing) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void ChangesReturnTypeOfSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(Boolean newParam, String existing) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public String someMethod(Boolean newParam, String existing) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public String someMethod(Boolean newParam, String existing) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void AddsParametersWithAnnotationsToEmptySignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod() {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(@annotation String newParam) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(@annotation String newParam) {
        // my special implementation
    }
}", result);
        }

        [Fact]
        public void AddsParametersWithAnnotationsToSignature()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(@annotation String existing) {
        // my special implementation
    }
}", outputContent: @"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(@annotation String existing, @annotation String newParam) {
    }
}");
            Assert.Equal(@"
public class EmptyClass {
    @IntentIgnoreBody()
    public void someMethod(@annotation String existing, @annotation String newParam) {
        // my special implementation
    }
}", result);
        }
    }
}
