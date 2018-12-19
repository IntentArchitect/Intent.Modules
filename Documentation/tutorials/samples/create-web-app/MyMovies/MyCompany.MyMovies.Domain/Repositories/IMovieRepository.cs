using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.Framework.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.Interface", Version = "1.0")]

namespace MyCompany.MyMovies.Domain
{
    public interface IMovieRepository : IRepository<IMovie, Movie>
    {
        IMovie FindById(Guid id);
        Task<IMovie> FindByIdAsync(Guid id);
    }
}