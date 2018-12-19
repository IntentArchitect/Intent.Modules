using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;
using MyCompany.MyMovies.Domain.Common;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.DomainEntityState", Version = "1.0")]

namespace MyCompany.MyMovies.Domain
{

    public partial class Movie : Object, IMovie
    {
        public Movie()
        {
        }

        private Guid? _id = null;

        /// <summary>
        /// Get the persistent object's identifier
        /// </summary>
        public virtual Guid Id
        {
            get { return _id ?? (_id = IdentityGenerator.NewSequentialId()).Value; }
            set { _id = value; }
        }

        private string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
            }
        }

        private System.DateTime _releaseDate;

        public System.DateTime ReleaseDate
        {
            get { return _releaseDate; }
            set
            {
                _releaseDate = value;
            }
        }

        private string _genre;

        public string Genre
        {
            get { return _genre; }
            set
            {
                _genre = value;
            }
        }

        private decimal _price;

        public decimal Price
        {
            get { return _price; }
            set
            {
                _price = value;
            }
        }
    }
}
