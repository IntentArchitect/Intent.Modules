using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityInterface", Version = "1.0")]

namespace MyCompany.MyMovies.Domain
{

    public partial interface IMovie
    {

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        Guid Id { get; }

        string Title { get; set; }

        System.DateTime ReleaseDate { get; set; }

        string Genre { get; set; }

        decimal Price { get; set; }

    }
}
