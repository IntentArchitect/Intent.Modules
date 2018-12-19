using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MyCompany.MyMovies.Domain;
using MyCompany.MyMovies.Application;
using System;

[assembly: DefaultIntentManaged(Mode.Merge)]
[assembly: IntentTemplate("Intent.Application.ServiceImplementations", Version = "1.0")]

namespace MyCompany.MyMovies.Application.ServiceImplementation
{
    public class MovieManager : IMovieManager
    {
        private readonly IMovieRepository _movieRepository;

        public MovieManager(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task Create(MovieDTO dto)
        {
            _movieRepository.Add(new Movie()
            {
                Title = dto.Title,
                ReleaseDate = dto.ReleaseDate,
                Genre = dto.Genre,
                Price = dto.Price
            });
        }

        [IntentManaged(Mode.Merge, Body = Mode.Ignore, Signature = Mode.Fully)]
        public async Task<List<MovieDTO>> List()
        {
            var movies = await _movieRepository.FindAllAsync();
            return movies.Select(x => MovieDTO.Create(
                title: x.Title,
                releaseDate: x.ReleaseDate,
                genre: x.Genre,
                price: x.Price)).ToList();
        }

        public void Dispose()
        {
        }
    }
}