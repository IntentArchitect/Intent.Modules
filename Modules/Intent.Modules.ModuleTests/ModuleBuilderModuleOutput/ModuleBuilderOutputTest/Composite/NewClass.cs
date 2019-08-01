using System;
using Intent.RoslynWeaver.Attributes;
using ModuleBuilderOutputTest.DependantA;
// Mode.Fully will overwrite file on each run. 
// Add in explicit [IntentManaged.Ignore] attributes to class or methods. Alternatively change to Mode.Merge (additive) or Mode.Ignore (once-off)
[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilderTests.Composite", Version = "1.0")]

namespace ModuleBuilderOutputTest.Composite
{
    public class NewClass
    {
        public NewClassDependant DependantA { get; }
    }
}