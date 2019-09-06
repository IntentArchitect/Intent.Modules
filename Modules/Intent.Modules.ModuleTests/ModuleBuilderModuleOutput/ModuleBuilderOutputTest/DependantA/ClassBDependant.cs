using System;
using Intent.RoslynWeaver.Attributes;

// Mode.Fully will overwrite file on each run. 
// Add in explicit [IntentManaged.Ignore] attributes to class or methods. Alternatively change to Mode.Merge (additive) or Mode.Ignore (once-off)
[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("ModuleBuilderTests.DependantA", Version = "1.0")]

namespace ModuleBuilderOutputTest.DependantA
{
    public class ClassBDependant
    {

    }
}