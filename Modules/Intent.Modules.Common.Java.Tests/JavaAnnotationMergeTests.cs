using System;
using Intent.Modules.Common.Java.Weaving;
using Xunit;

namespace Intent.Modules.Common.Java.Tests
{
    public class JavaAnnotationMergeTests
    {
        [Fact]
        public void AddsAnnotationToEmptyClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: ClassWithNoAnnotations, outputContent: ClassWithOneAnnotation);
            Assert.Equal(ClassWithOneAnnotation, result);
        }

        [Fact]
        public void AddsAnnotationToEmptyClassMerged()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: EmptyClassMerged, outputContent: ClassWithOneAnnotation);
            Assert.Equal(@"
@OneAnnotation(""one of one"")
@IntentMerge()
public class EmptyClass {

    @IntentIgnore()
    public void someMethod() { // prevent straight override
    }
}", result);
        }

        [Fact]
        public void UpdatesAnnotations()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: ClassWithOneAnnotation, outputContent: ClassTwoAnnotations);
            Assert.Equal(@"
@OneAnnotation(""one of two"")
@TwoAnnotation(""two of two"")
public class EmptyClass {
    @IntentIgnore()
    public void someMethod() { // prevent straight override
    }
}", result);
        }

        [Fact]
        public void SkipsExistingAndAddsAnnotationsOnMergedClass()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: MergedClassWithTwoDifferentAnnotations, outputContent: ClassTwoAnnotations);
            Assert.Equal(@"
@OneAnnotation(""one of two"")
@TwoAnnotation(""two of two"")
@OneDifferentAnnotation(""one of two"")
@TwoDifferentAnnotation(""two of two"")
@IntentMerge()
public class EmptyClass {
    @IntentIgnore()
    public void someMethod() { // prevent straight override
    }
}", result);
        }

        [Fact]
        public void AddsAnnotationToMethodWithNoAnnotations()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: MethodWithNoAnnotations, outputContent: MergedMethodWithOneAnnotation);
            Assert.Equal(MergedMethodWithOneAnnotation, result);
        }

        [Fact]
        public void AddsAnnotationToMethodWithOneAnnotations()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: MergedMethodWithOneAnnotation, outputContent: MergedMethodWithTwoAnnotation);
            Assert.Equal(MergedMethodWithTwoAnnotation, result);
        }

        [Fact]
        public void SkipsRemovingAnnotationFromMethodWhenMerged()
        {
            var merger = new JavaWeavingMerger();
            var result = merger.Merge(existingContent: MergedMethodWithTwoAnnotation, outputContent: MergedMethodWithOneAnnotation);
            Assert.Equal(MergedMethodWithTwoAnnotation, result);
        }


        public static string ClassWithNoAnnotations = @"
public class EmptyClass {
    @IntentIgnore()
    public void someMethod() { // prevent straight override
    }
}";

        public static string EmptyClassMerged = @"
@IntentMerge()
public class EmptyClass {
}";

        public static string ClassWithOneAnnotation = @"
@OneAnnotation(""one of one"")
public class EmptyClass {
    @IntentIgnore()
    public void someMethod() { // prevent straight override
    }
}";

        public static string ClassTwoAnnotations = @"
@OneAnnotation(""one of two"")
@TwoAnnotation(""two of two"")
public class EmptyClass {
    @IntentIgnore()
    public void someMethod() { // prevent straight override
    }
}";

        public static string MergedClassWithTwoDifferentAnnotations = @"
@OneDifferentAnnotation(""one of two"")
@TwoDifferentAnnotation(""two of two"")
@IntentMerge()
public class EmptyClass {
    @IntentIgnore()
    public void someMethod() { // prevent straight override
    }
}";

        public static string MethodWithNoAnnotations = @"
@IntentMerge
public class EmptyClass {
    public void someMethod() {
        // some implementation
    }
}";
        public static string MergedMethodWithOneAnnotation = @"
@IntentMerge
public class EmptyClass {
    @OneAnnotation()
    @IntentMerge()
    public void someMethod() {
        // some implementation
    }
}";

        public static string MergedMethodWithTwoAnnotation = @"
@IntentMerge
public class EmptyClass {
    @OneAnnotation()
    @TwoAnnotation()
    @IntentMerge()
    public void someMethod() {
        // some implementation
    }
}";
    }
}
