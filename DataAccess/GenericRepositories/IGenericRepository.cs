using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.GenericRepositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllWithInclude(Expression<Func<T, object>> includes);
        Task<IEnumerable<T>> GetAllWithOrderByDescending(Expression<Func<T, int>> orderBy);
        Task<IEnumerable<T>> GetAllWithCondition(Expression<Func<T, bool>> expression = null, List<Expression<Func<T, object>>> includes = null, Expression<Func<T, int>> orderBy = null, bool disableTracking = true);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> filter);

        Task<IEnumerable<T>> GetAllWithPagination(Expression<Func<T, bool>> expression = null, List<Expression<Func<T, object>>> includes = null, Expression<Func<T, int>> orderBy = null, bool disableTracking = true, int? page = null, int? pageSize = null);
        Task AddRangeAsync(List<T> entities);
        Task<int> SaveChangesAsync();
        Task<int> CountAll(Expression<Func<T, bool>> expression = null);
        Task<T> GetById(object id);
        Task<T> GetByWithCondition(Expression<Func<T, bool>> expression = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true);
        Task<IEnumerable<T>> GetByCondition(Expression<Func<T, bool>> expression);
        Task Insert(T obj);
        Task Delete(T obj);
        Task Update(T obj);
        Task Save();

        Task<T> GetAsync(Guid? id);
        Task<TResult> GetAsync<TResult>(Guid? id);
        Task<List<T>> GetAllAsync();
        Task<List<TResult>> GetAllAsync<TResult>();
        Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
        Task<T> AddAsync(T entity);
        Task<TResult> AddAsync<TSource, TResult>(TSource source);
        Task DeleteAsync(Guid id);
        Task UpdateAsync<TSource>(Guid id, TSource source) where TSource : class;
        /*        Task UpdateAsync<TSource>(Guid id, TSource source) where TSource : IBaseDto;
        */          
        Task UpdateAsync(Guid id, T source);
        Task UpdateAsync(T entity);
        Task<bool> Exists(Guid id);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);


    }
}
