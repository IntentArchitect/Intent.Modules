using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.DTO", Version = "1.0")]

namespace MyCompany.MyMovies.Application
{
    [DataContract]
    public class MovieDTO
    {
        public MovieDTO()
        {
        }

        public static MovieDTO Create(
            string title,
            System.DateTime releaseDate,
            string genre,
            decimal price)
        {
            return new MovieDTO
            {
                Title = title,
                ReleaseDate = releaseDate,
                Genre = genre,
                Price = price,
            };
        }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public System.DateTime ReleaseDate { get; set; }

        [DataMember]
        public string Genre { get; set; }

        [DataMember]
        public decimal Price { get; set; }
    }
}