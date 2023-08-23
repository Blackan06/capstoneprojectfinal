using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessObjects.Model;
using DataAccess.Exceptions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace DataAccess.GenericRepositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly db_a9c31b_capstoneContext _dbContext;
        private readonly IMapper _mapper;

        public GenericRepository(db_a9c31b_capstoneContext dbContext,IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<int> CountAll(Expression<Func<T, bool>> expression = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (expression != null) query = query.Where(expression);
            return await query.CountAsync();
        }

        public async Task Delete(T obj)
        {
            _dbContext.Set<T>().Remove(obj);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var lst = await _dbContext.Set<T>().ToListAsync();
            if (lst.Count <= 0)
            {
                return null;
            }
            return lst;
        }

        public async Task<IEnumerable<T>> GetAllWithCondition(Expression<Func<T, bool>> expression = null, List<Expression<Func<T, object>>> includes = null, Expression<Func<T, int>> orderBy = null, bool disableTracking = true)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
            if (expression != null) query = query.Where(expression);
            if (orderBy != null)
                return await query.OrderByDescending(orderBy).ToListAsync();
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllWithInclude(Expression<Func<T, object>> includes)
        {
            var lst = await _dbContext.Set<T>().Include(includes).ToListAsync();
            if (lst.Count <= 0)
            {
                return null;
            }
            return lst;
        }

        public async Task<IEnumerable<T>> GetAllWithOrderByDescending(Expression<Func<T, int>> orderBy)
        {
            var lst = await _dbContext.Set<T>().OrderByDescending(orderBy).ToListAsync();
            if (lst.Count <= 0)
            {
                return null;
            }
            return lst;
        }

        public async Task<IEnumerable<T>> GetAllWithPagination(Expression<Func<T, bool>> expression = null, List<Expression<Func<T, object>>> includes = null, Expression<Func<T, int>> orderBy = null, bool disableTracking = true, int? page = null, int? pageSize = null)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
            if (expression != null) query = query.Where(expression);
            if (orderBy != null)
                return await query.OrderByDescending(orderBy).Skip(((int)page - 1) * (int)pageSize)
                        .Take((int)pageSize).ToListAsync();
            return await query.Skip(((int)page - 1) * (int)pageSize)
                        .Take((int)pageSize).ToListAsync();
        }

        public async Task<IEnumerable<T>> GetByCondition(Expression<Func<T, bool>> expression)
        {
            var lst = await _dbContext.Set<T>().Where(expression).ToListAsync();
            if (lst.Count <= 0)
            {
                return null;
            }
            return lst;
        }

        public async Task<T> GetById(object id)
        {
            var rs = await _dbContext.Set<T>().FindAsync(id);
            if (rs == null)
            {
                return null;
            }
            return rs;
        }

        public async Task<T> GetByWithCondition(Expression<Func<T, bool>> expression = null, List<Expression<Func<T, object>>> includes = null, bool disableTracking = true)
        {
            IQueryable<T> query = _dbContext.Set<T>();
            if (disableTracking) query = query.AsNoTracking();
            if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
            if (expression != null) query = query.Where(expression);
            return await query.FirstOrDefaultAsync();
        }

        public async Task Insert(T obj)
        {
            _dbContext.Set<T>().Add(obj);
            await _dbContext.SaveChangesAsync();
        }

        public async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task Update(T obj)
        {
            _dbContext.ChangeTracker.Clear();
            _dbContext.Set<T>().Update(obj);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public async Task<TResult> AddAsync<TSource, TResult>(TSource source)
        {
            var entity = _mapper.Map<T>(source);

            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<TResult>(entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetAsync(id);

            if (entity is null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }

            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> Exists(Guid id)
        {
            var entity = await GetAsync(id);
            return entity != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public async Task<PagedResult<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters)
        {
            var totalSize = await _dbContext.Set<T>().CountAsync();
            var items = await _dbContext.Set<T>()
                .Skip(queryParameters.StartIndex)
                .Take(queryParameters.PageSize)
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return new PagedResult<TResult>
            {
                Items = items,
                PageNumber = queryParameters.PageNumber,
                RecordNumber = queryParameters.PageSize,
                TotalCount = totalSize
            };
        }

        public async Task<List<TResult>> GetAllAsync<TResult>()
        {
            return await _dbContext.Set<T>()
                .ProjectTo<TResult>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }
        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> filter)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(filter);
        }
        public async Task<T> GetAsync(Guid? id)
        {
            var result = await _dbContext.Set<T>().FindAsync(id);
            if (result is null)
            {
                throw new NotFoundException(typeof(T).Name, id.HasValue ? id : "No Key Provided");
            }

            return result;
        }

        public async Task<TResult> GetAsync<TResult>(Guid? id)
        {
            var result = await _dbContext.Set<T>().FindAsync(id);
            if (result is null)
            {
                throw new NotFoundException(typeof(T).Name, id.HasValue ? id : "No Key Provided");
            }

            return _mapper.Map<TResult>(result);
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync<TSource>(Guid id, TSource source) where TSource : class
        {
            var entity = await GetAsync(id);

            if (entity == null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }

            _mapper.Map(source, entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            try
            {
                // Lưu thay đổi vào cơ sở dữ liệu
                await _dbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý các trường hợp khi có lỗi cạnh tranh khi lưu thay đổi vào cơ sở dữ liệu
                // Ví dụ: một người dùng khác cập nhật thực thể cùng lúc.
                throw;
            }
        }
        
        public async Task UpdateAsync(Guid id, T source)
        {
            T entity = await GetAsync(id);
            if (entity == null)
            {
                throw new NotFoundException(typeof(T).Name, id);
            }

            _mapper.Map(source, entity);
            await _dbContext.SaveChangesAsync();
        }
        public async Task AddRangeAsync(List<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

        }

        public async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Set<T>().AnyAsync(expression);
        }

    }
}
