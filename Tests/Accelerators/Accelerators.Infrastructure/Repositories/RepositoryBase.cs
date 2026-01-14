using System.Linq.Expressions;
using Accelerators.Application.Common.Pagination;
using Accelerators.Domain.Common.Interfaces;
using Accelerators.Domain.Repositories;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.EntityFrameworkCore;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.EntityFrameworkCore.Repositories.RepositoryBase", Version = "1.0")]

namespace Accelerators.Infrastructure.Repositories
{
    public class RepositoryBase<TDomain, TPersistence, TDbContext> : IEFRepository<TDomain, TPersistence>
        where TDbContext : DbContext, IUnitOfWork
        where TPersistence : class, TDomain
        where TDomain : class
    {
        protected readonly TDbContext _dbContext;
        private readonly IMapper _mapper;

        public RepositoryBase(TDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mapper = mapper;
        }

        public IUnitOfWork UnitOfWork => _dbContext;

        public virtual void Remove(TDomain entity)
        {
            GetSet().Remove((TPersistence)entity);
        }

        public virtual void Add(TDomain entity)
        {
            GetSet().Add((TPersistence)entity);
        }

        public virtual void Update(TDomain entity)
        {
            GetSet().Update((TPersistence)entity);
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<TDomain?> FindAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, queryOptions).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<TDomain?> FindAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).SingleOrDefaultAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression: null).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression, queryOptions).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression: null);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(filterExpression, queryOptions);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<List<TDomain>> FindAllAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).ToListAsync<TDomain>(cancellationToken);
        }

        public virtual async Task<IPagedList<TDomain>> FindAllAsync(
            int pageNo,
            int pageSize,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var query = QueryInternal(queryOptions);
            return await ToPagedListAsync<TDomain>(
                query,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).CountAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(filterExpression).AnyAsync(cancellationToken);
        }

        public virtual async Task<bool> AnyAsync(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions = default,
            CancellationToken cancellationToken = default)
        {
            return await QueryInternal(queryOptions).AnyAsync(cancellationToken);
        }

        public async Task<List<TProjection>> FindAllProjectToAsync<TProjection>(CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression: null);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await projection.ToListAsync(cancellationToken);
        }

        public async Task<List<TProjection>> FindAllProjectToAsync<TProjection>(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(queryOptions);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await projection.ToListAsync(cancellationToken);
        }

        public async Task<List<TProjection>> FindAllProjectToAsync<TProjection>(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await projection.ToListAsync(cancellationToken);
        }

        public async Task<List<TProjection>> FindAllProjectToAsync<TProjection>(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression, queryOptions);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await projection.ToListAsync(cancellationToken);
        }

        public async Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression: null);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await ToPagedListAsync(
                projection,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public async Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(
            int pageNo,
            int pageSize,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(queryOptions);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await ToPagedListAsync(
                projection,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public async Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await ToPagedListAsync(
                projection,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public async Task<IPagedList<TProjection>> FindAllProjectToAsync<TProjection>(
            Expression<Func<TPersistence, bool>> filterExpression,
            int pageNo,
            int pageSize,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression, queryOptions);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await ToPagedListAsync(
                projection,
                pageNo,
                pageSize,
                cancellationToken);
        }

        public async Task<TProjection?> FindProjectToAsync<TProjection>(
            Expression<Func<TPersistence, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await projection.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TProjection?> FindProjectToAsync<TProjection>(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(filterExpression, queryOptions);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await projection.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<TProjection?> FindProjectToAsync<TProjection>(
            Func<IQueryable<TPersistence>, IQueryable<TPersistence>> queryOptions,
            CancellationToken cancellationToken = default)
        {
            var queryable = QueryInternal(queryOptions);
            var projection = queryable.ProjectTo<TProjection>(_mapper.ConfigurationProvider);
            return await projection.FirstOrDefaultAsync(cancellationToken);
        }

        protected virtual IQueryable<TPersistence> QueryInternal(Expression<Func<TPersistence, bool>>? filterExpression)
        {
            var queryable = CreateQuery();
            if (filterExpression != null)
            {
                queryable = queryable.Where(filterExpression);
            }
            return queryable;
        }

        protected virtual IQueryable<TResult> QueryInternal<TResult>(
            Expression<Func<TPersistence, bool>> filterExpression,
            Func<IQueryable<TPersistence>, IQueryable<TResult>> queryOptions)
        {
            var queryable = CreateQuery();
            queryable = queryable.Where(filterExpression);
            var result = queryOptions(queryable);
            return result;
        }

        protected virtual IQueryable<TPersistence> QueryInternal(Func<IQueryable<TPersistence>, IQueryable<TPersistence>>? queryOptions)
        {
            var queryable = CreateQuery();
            if (queryOptions != null)
            {
                queryable = queryOptions(queryable);
            }
            return queryable;
        }

        protected virtual IQueryable<TPersistence> CreateQuery()
        {
            return GetSet();
        }

        protected virtual DbSet<TPersistence> GetSet()
        {
            return _dbContext.Set<TPersistence>();
        }

        private static async Task<IPagedList<T>> ToPagedListAsync<T>(
            IQueryable<T> queryable,
            int pageNo,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var count = await queryable.CountAsync(cancellationToken);
            var skip = ((pageNo - 1) * pageSize);

            var results = await queryable
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
            return new PagedList<T>(count, pageNo, pageSize, results);
        }
    }
}