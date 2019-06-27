using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial interface IH_OptionalAggregateNav
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }

        ICollection<H_MultipleDependent> H_MultipleDependents { get; set; }

    }
}
