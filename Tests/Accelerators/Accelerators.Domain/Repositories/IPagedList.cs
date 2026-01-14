using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Entities.Repositories.Api.PagedListInterface", Version = "1.0")]

namespace Accelerators.Domain.Repositories
{
    /// <summary>
    /// Instead of retrieving the entire collection of elements from
    /// a persistence store, a single <see cref="IPagedList{T}"/> is returned
    /// representing a single "page" of elements. Supplying a different <b>PageNo</b>
    /// will return a different "page" of elements.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    public interface IPagedList<T> : IList<T>
    {
        int TotalCount { get; }
        int PageCount { get; }
        int PageNo { get; }
        int PageSize { get; }
    }
}
