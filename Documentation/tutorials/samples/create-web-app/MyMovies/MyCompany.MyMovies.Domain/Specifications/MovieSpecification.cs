using System;
using System.Collections.Generic;
using System.Linq;
using Intent.Framework.Domain.Specification;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Ignore)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.EntitySpecification", Version = "1.0")]

namespace MyCompany.MyMovies.Domain
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class MovieSpecification : DomainSpecificationBase<Movie, MovieSpecification>
    {
        [IntentManaged(Mode.Fully)]
        public static MovieSpecification Where()
        {
            return new MovieSpecification();
        }
    }
}