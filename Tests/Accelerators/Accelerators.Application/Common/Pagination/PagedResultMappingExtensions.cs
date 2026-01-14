using Accelerators.Domain.Repositories;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Dtos.Pagination.PagedResultMappingExtensions", Version = "1.0")]

namespace Accelerators.Application.Common.Pagination
{
    public static class PagedResultMappingExtensions
    {
        /// <summary>
        /// For mapping a paged-list of Domain elements into a page of DTO elements. See <see cref="IPagedList{T}"/>.
        /// </summary>
        /// <param name="pagedList">A single page retrieved from a persistence store.</param>
        /// <param name="mapFunc">
        /// Provide a mapping function where a single Domain element is supplied to the function
        /// that returns a single DTO element. There are some convenient mapping extension methods
        /// available, or alternatively you can instantiate a new DTO element.
        /// <example>results.MapToPagedResult(x => x.MapToItemDTO(_mapper));</example>
        /// <example>results.MapToPagedResult(x => ItemDTO.Create(x.ItemName));</example>
        /// </param>
        /// <typeparam name="TDomain">Domain element type.</typeparam>
        /// <typeparam name="TDto">DTO element type.</typeparam>
        /// <returns>A single page of DTO elements.</returns>
        public static PagedResult<TDto> MapToPagedResult<TDomain, TDto>(
            this IPagedList<TDomain> pagedList,
            Func<TDomain, TDto> mapFunc)
        {
            var data = pagedList.Select(mapFunc).ToList();
            return PagedResult<TDto>.Create(
                totalCount: pagedList.TotalCount,
                pageCount: pagedList.PageCount,
                pageSize: pagedList.PageSize,
                pageNumber: pagedList.PageNo,
                data: data);
        }

        /// <summary>
        /// For mapping a paged-list of Domain elements into a page of DTO elements. See <see cref="IPagedList{T}"/>.
        /// </summary>
        /// <param name="pagedList">A single page retrieved from a persistence store.</param>
        /// <typeparam name="TDto">DTO element type.</typeparam>
        /// <returns>A single page of DTO elements.</returns>
        public static PagedResult<TDto> MapToPagedResult<TDto>(this IPagedList<TDto> pagedList)
        {
            return PagedResult<TDto>.Create(
                totalCount: pagedList.TotalCount,
                pageCount: pagedList.PageCount,
                pageSize: pagedList.PageSize,
                pageNumber: pagedList.PageNo,
                data: pagedList);
        }
    }
}