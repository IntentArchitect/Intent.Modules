using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.Entities.DomainEntity", Version = "1.0")]

namespace MyCompany.MyMovies.Domain
{
    [IntentManaged(Mode.Merge)]
    [DefaultIntentManaged(Mode.Merge, Signature = Mode.Merge, Body = Mode.Merge, Targets = Targets.Methods, AccessModifiers = AccessModifiers.Public)]
    public partial class Movie
    {

    }
}