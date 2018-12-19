using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Contracts.ServiceContract", Version = "1.0")]

namespace MyCompany.MyMovies.Application
{

    public interface IMovieManager : IDisposable
    {

        Task Create(MovieDTO dto);

        Task<List<MovieDTO>> List();
    }
}