using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace EfEntityGenerationTests.Domain
{

    public partial interface IM_SelfReferenceBiNav
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }
        Guid? SelfId { get; }

        M_SelfReferenceBiNav Self { get; set; }
        Guid? OriginalId { get; }

        M_SelfReferenceBiNav Original { get; set; }

    }
}
