using System;
using System.Linq;
using System.Threading.Tasks;
using Intent.Framework.Core.Context;
using Intent.Framework.Domain.Repositories;
using Intent.Framework.EntityFrameworkCore;
using Intent.Framework.EntityFrameworkCore.Repositories;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;
using MyCompany.MyMovies.Domain;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.Implementation", Version = "1.0")]

namespace MyCompany.MyMovies.Infrastructure.Data
{
    [IntentManaged(Mode.Merge)]
    public class MovieRepository : RepositoryBase<IMovie, Movie, MyMoviesDbContext>, IMovieRepository
    {
        public MovieRepository(MyMoviesDbContext dbContext) : base(dbContext)
        {
        }

        public IMovie FindById(Guid id)
        {
            return CreateQuery().SingleOrDefault(x => x.Id == id);
        }

        public async Task<IMovie> FindByIdAsync(Guid id)
        {
            return await CreateQuery().SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}