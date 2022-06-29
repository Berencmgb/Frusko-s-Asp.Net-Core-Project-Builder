namespace ASP.NET.Core_Project_Builder
{
    public static class Boilerplate
    {
        public const string BaseEntityTemplate =
@"namespace {namespace}
{
    public class BaseEntity : IBaseEntity
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public bool IsDeleted { get; set; }
    }

    public interface IBaseEntity
    {
        public int Id { get; set; }
        public string Reference { get; set; }
        public bool IsDeleted { get; set; }
    }
}";

        public const string BaseDTOTemplate =
@"namespace {namespace}
{
    public class BaseDTO
    {
        public string Reference { get; set; }
        public bool IsDeleted { get; set; }
    }
}";

        public const string PaginationTemplate =
@"namespace {namespace}
{
    public class Pagination
    {
        public int PageSize { get; set; } = 10;
        public int Page { get; set; } = 0;

    }
}";

        public const string ResultTemplate =
@"namespace {namespace}
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Value { get; set; }
    }

    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}";

        public const string UserTemplate =
@"using Microsoft.AspNetCore.Identity;

namespace {namespace}
{
    public class User : IdentityUser<int>, IBaseEntity
    {
        public string Reference { get; set; }
        public bool IsDeleted { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}";

        public const string SharedDBContext =
@"using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace {namespace}
{

    public class SharedDBContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public SharedDBContext(DbContextOptions<SharedDBContext> options) : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach(var added in ChangeTracker.Entries<IBaseEntity>().Where(a => a.State == EntityState.Added))
            {
                if (!string.IsNullOrEmpty(added.Entity.Reference))
                    continue;

                added.Entity.Reference = Guid.NewGuid().ToString();
            }

            return base.SaveChangesAsync(cancellationToken);    
        }
    }
}";

        public const string BaseRepositoryTemplate =
@"using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace {namespace}
{
    public class BaseRepository<TEntity, TRepository> : IBaseRepository<TEntity>, IDisposable where TEntity : class, IBaseEntity
    {
        public AppDbContext Context;
        public DbSet<TEntity> Table;
        protected readonly IMapper _mapper;

        public List<string> Includes { get; set; } = new List<string>();
        protected ILogger<TRepository> _logger;

        public BaseRepository(AppDbContext context, ILogger<TRepository> logger,
            IMapper mapper)
        {
            Context = context;
            Table = Context.Set<TEntity>();
            Table.IgnoreAutoIncludes();
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result> DeleteAsync(TEntity entity)
        {
            var result = new Result { };
            try
            {
                Table.Remove(entity);
                await SaveChangesAsync();
                result.Success = true;
                result.Message = $""Entity Removed From Table"";
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
                return result;
        }
    
        public async Task<Result<List<TEntity>>> GetAllAsync()
        {
            var result = new Result<List<TEntity>> { };
        
            try
            {
                if (Includes == null || Includes.Count == 0)
                {
                    result.Value = await Table.ToListAsync();
                }
                else
                {
                    var entities = Table.AsQueryable();
                    foreach (var include in Includes)
                        entities = entities.Include(include);
        
                    result.Value = await entities.ToListAsync();
                }
        
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task<Result<List<TEntity>>> GetAllAsync(Pagination pagination)
        {
            var result = new Result<List<TEntity>> { };
        
            try
            {
                if (Includes == null)
                    result.Value = await Table.Skip(pagination.PageSize * pagination.Page).Take(pagination.PageSize).ToListAsync();
        
                else
                {
                    var entities = Table.AsQueryable();
                    foreach (var include in Includes)
                        entities = entities.Include(include);
        
                    result.Value = await entities.Skip(pagination.PageSize * pagination.Page).Take(pagination.PageSize).ToListAsync();
                }
        
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression)
        {
            var result = new Result<List<TEntity>> { };
        
            try
            {
                if (Includes == null)
                    result.Value = await Table.Where(expression).ToListAsync();
        
                else
                {
                    var entities = Table.Where(expression).AsQueryable();
                    foreach (var include in Includes)
                        entities = entities.Include(include);
        
                    result.Value = await entities.ToListAsync();
                }
        
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, Pagination pagination)
        {
            var result = new Result<List<TEntity>> { };
        
            try
            {
                if (Includes == null)
                    result.Value = await Table.Where(expression).Skip(pagination.PageSize * pagination.Page).Take(pagination.PageSize).ToListAsync();
        
                else
                {
                    var entities = Table.Where(expression).Skip(pagination.PageSize * pagination.Page).Take(pagination.PageSize).AsQueryable();
                    foreach (var include in Includes)
                        entities = entities.Include(include);
        
                    result.Value = await entities.ToListAsync();
                }
        
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression)
        {
            var result = new Result<List<TEntity>> { };
        
            try
            {
                if (Includes == null)
                    result.Value = await Table.Where(expression).ToListAsync();
        
                else
                {
                    var entities = Table.Where(expression).AsQueryable();
                    foreach (var include in Includes)
                        entities = entities.Include(include);
        
                    result.Value = await entities.ToListAsync();
                }
        
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination pagination)
        {
            var result = new Result<List<TEntity>> { };
        
            try
            {
                if (Includes == null)
                    result.Value = await Table.Where(expression).Skip(pagination.PageSize * pagination.Page).Take(pagination.PageSize).ToListAsync();
        
                else
                {
                    var entities = Table.Where(expression).Skip(pagination.PageSize * pagination.Page).Take(pagination.PageSize).AsQueryable();
                    foreach (var include in Includes)
                        entities = entities.Include(include);
        
                    result.Value = await entities.ToListAsync();
                }
        
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task<TEntity> GetSingleWhereAsync(string expression)
        {
            if (Includes == null)
                return await Table.FirstOrDefaultAsync(expression);
        
            var result = Table.AsQueryable();
            foreach (var include in Includes)
            {
                result = result.Include(include);
            }
        
            return await result.FirstOrDefaultAsync(expression);
        }
        
        public async Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression)
        {
            if (Includes == null)
                return await Table.FirstOrDefaultAsync(expression);
        
            var result = Table.AsQueryable();
            foreach (var include in Includes)
            {
                result = result.Include(include);
            }
        
            return await result.FirstOrDefaultAsync(expression);
        }
        
        public virtual async Task<Result<TEntity>> InsertAsync(TEntity entity)
        {
            var result = new Result<TEntity> { };
            result.Value = entity;
            try
            {
                if (string.IsNullOrWhiteSpace(entity.Reference))
                    entity.Reference = Guid.NewGuid().ToString();
        
                await Table.AddAsync(entity);
                Context.Entry(entity).State = EntityState.Added;
                await SaveChangesAsync();
                result.Message = entity.Reference;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task SaveChangesAsync()
        {
            await Context.SaveChangesAsync();
        }
        
        public async Task<Result<TEntity>> UpdateAsync(TEntity entity)
        {
            var result = new Result<TEntity> { };
            result.Value = entity;
            try
            {
                Context.Attach(entity);
                Context.Update(entity).State = EntityState.Modified;
                await SaveChangesAsync();
                result.Message = entity.Reference;
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
        
            return result;
        }
        
        public async Task<Result> DeleteAsync(string reference)
        {
            var result = new Result { };
            try
            {
                Table.FirstOrDefault(e => e.Reference == reference).IsDeleted = true;
                await SaveChangesAsync();
                result.Success = true;
                result.Message = $""Entity marked as deleted"";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
            return result;
        }
        
        public async Task<Result> DeleteFromTableAsync(string reference)
        {
            var result = new Result { };
            try
            {
                Table.Remove(Table.FirstOrDefault(e => e.Reference == reference));
                await SaveChangesAsync();
                result.Success = true;
                result.Message = $""Entity Removed From Table"";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                result.Success = false;
                result.Message = $""Exception Thrown. Error: {ex.Message}"";
            }
            return result;
        }

        private bool _disposed = false;
        
        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                    Context.Dispose();
            }
        
            _disposed = true;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public interface IBaseRepository<TEntity> where TEntity : IBaseEntity
    {
        public Task<Result<List<TEntity>>> GetAllAsync();
        public Task<Result<List<TEntity>>> GetAllAsync(Pagination pagination);
        public Task<Result<List<TEntity>>> GetAllWhereAsync(string expression);
        public Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, Pagination pagination);
        public Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression);
        public Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination pagination);
        public Task<TEntity> GetSingleWhereAsync(string expression);
        public Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression);
        public Task<Result<TEntity>> InsertAsync(TEntity entity);
        public Task<Result<TEntity>> UpdateAsync(TEntity entity);
        public Task<Result> DeleteAsync(TEntity entity);
        public Task<Result> DeleteAsync(string reference);
        public Task<Result> DeleteFromTableAsync(string reference);
        public Task SaveChangesAsync();
        public List<string> Includes { get; set; }
    
    }
}";

        public const string BaseServiceTemplate =
@"using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace {namespace}
{
    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, IBaseEntity
    {
        public IBaseRepository<TEntity> Repository { get; set; }


        public BaseService(IBaseRepository<TEntity> repository)
        {
            Repository = repository;
        }

        #region GetAll

        public async virtual Task<Result<List<TEntity>>> GetAllAsync()
        {
            var result = await Repository.GetAllAsync();

            return result;
        }

        public async Task<Result<List<TEntity>>> GetAllAsync(List<string> includes)
        {
            foreach (var include in includes)
            {
                Repository.Includes.Add(include);
            }
            return await Repository.GetAllAsync();
        }

        public async Task<Result<List<TEntity>>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
        {
            Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
            return await Repository.GetAllAsync();
        }

        public async Task<Result<List<TEntity>>> GetAllAsync(Pagination page)
        {
            return await Repository.GetAllAsync(page);
        }

        public async Task<Result<List<TEntity>>> GetAllAsync(Pagination page, List<string> includes)
        {
            foreach (var include in includes)
            {
                Repository.Includes.Add(include);
            }
            return await Repository.GetAllAsync(page);
        }

        public async Task<Result<List<TEntity>>> GetAllAsync(Pagination page, params Expression<Func<TEntity, object>>[] includes)
        {
            Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
            return await Repository.GetAllAsync(page);
        }

        #endregion

        #region GetAllWhere

        public async virtual Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, List<string> includes)
        {
            foreach (var include in includes)
            {
                Repository.Includes.Add(include);
            }
            return await Repository.GetAllWhereAsync(expression);
        }

        public async virtual Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
            return await Repository.GetAllWhereAsync(expression);
        }

        public async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await Repository.GetAllWhereAsync(expression);
        }

        public async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression)
        {
            return await Repository.GetAllWhereAsync(expression);
        }

        public async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination page, params Expression<Func<TEntity, object>>[] includes)
        {
            Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
            return await Repository.GetAllWhereAsync(expression, page);
        }

        public async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination page)
        {
            return await Repository.GetAllWhereAsync(expression, page);
        }

        public async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, List<string> includes, Pagination page)
        {
            foreach (var include in includes)
            {
                Repository.Includes.Add(include);
            }
            return await Repository.GetAllWhereAsync(expression, page);
        }

        public async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, Pagination page)
        {
            return await Repository.GetAllWhereAsync(expression, page);
        }

        #endregion

        #region GetSingleWhere

        public async virtual Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
            return await Repository.GetSingleWhereAsync(expression);
        }

        public async virtual Task<TEntity> GetSingleWhereAsync(string expression, List<string> includes)
        {
            foreach (var include in includes)
            {
                Repository.Includes.Add(include);
            }
            return await Repository.GetSingleWhereAsync(expression);
        }

        public async Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await Repository.GetSingleWhereAsync(expression);
        }

        public async Task<TEntity> GetSingleWhereAsync(string expression)
        {
            return await Repository.GetSingleWhereAsync(expression);
        }

        #endregion

        public async virtual Task<Result> DeleteAsync(TEntity entity)
        {
            return await Repository.DeleteAsync(entity);
        }

        public async virtual Task<Result<TEntity>> AddAsync(TEntity entity)
        {
            return await Repository.InsertAsync(entity);
        }

        public async virtual Task<Result<TEntity>> UpdateAsync(TEntity entity)
        {
            return await Repository.UpdateAsync(entity);
        }

        public async Task<Result> DeleteAsync(string reference)
        {
            return await Repository.DeleteAsync(reference);
        }
    }

    public interface IBaseService<TEntity> where TEntity : class, IBaseEntity
    {
        public Task<Result<List<TEntity>>> GetAllAsync();

        public Task<Result<List<TEntity>>> GetAllAsync(Pagination page);

        public Task<Result<List<TEntity>>> GetAllAsync(List<string> includes);

        public Task<Result<List<TEntity>>> GetAllAsync(Pagination page, List<string> includes);

        public Task<Result<List<TEntity>>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes);

        public Task<Result<List<TEntity>>> GetAllAsync(Pagination page, params Expression<Func<TEntity, object>>[] includes);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination page, params Expression<Func<TEntity, object>>[] includes);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination page);

        public Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes);

        public Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, List<string> includes);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, List<string> includes, Pagination page);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(string expression);

        public Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, Pagination page);

        public Task<TEntity> GetSingleWhereAsync(string expression, List<string> includes);

        public Task<TEntity> GetSingleWhereAsync(string expression);

        public Task<Result> DeleteAsync(string reference);

        public Task<Result> DeleteAsync(TEntity entity);

        public Task<Result<TEntity>> AddAsync(TEntity entity);

        public Task<Result<TEntity>> UpdateAsync(TEntity entity);


    }
}";

        public const string StringHelpersTemplate =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace {namespace}
{
    public static class StringHelper
    {
        public static List<string> GenerateIncludes(List<string> includes)
        {
            var result = new List<string>();

            foreach (var include in includes)
            {
                var includeString = include.ToString();
                var index = includeString.IndexOf('.');

                includeString = includeString.Remove(0, index + 1);

                if (includeString.Contains("".First()""))
                    includeString = includeString.Replace("".First()"", """");

                result.Add(includeString);
            }

            return result;
        }
    
        public static List<string> GenerateIncludes<TEntity>(params Expression<Func<TEntity, object>>[] includes)
        {
            var result = new List<string>();
        
            foreach (var include in includes)
            {
                var includeString = include.ToString();
                var index = includeString.IndexOf('.');
        
                includeString = includeString.Remove(0, index + 1);
        
                if (includeString.Contains("".First()""))
                    includeString = includeString.Replace("".First()"", """");
        
                result.Add(includeString);
            }
        
            return result;
        }

        public static bool IsBase64String(this string base64)
        {
            base64 = base64.Trim();
            return (base64.Length % 4 == 0) && Regex.IsMatch(base64, @""^[a-zA-Z0-9\+/]*={0,3}$"", RegexOptions.None);
        }

        public static string GenerateRandomCode(int length)
        {
            var random = new Random();
        
            const string chars = ""012A0BC1DE2FG3HI4JK5789LM6NO7PQ8RS9TUVWXYZ3456"";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}";

        public const string ExpressionHelpersTemplate =
@"using System.Linq.Expressions;

namespace {namespace}
{
    public static class ExpressionHelpers
    {
        public static Expression Simplify(this Expression expression)
        {
            var searcher = new DynamicExpressionSearcher();
            searcher.Visit(expression);
            return new DynamicExpressionEvaluator(searcher.DynamicExpressions).Visit(expression);
        }

        public static Expression<T> Simplify<T>(this Expression<T> expression)
        {
            return (Expression<T>)Simplify((Expression)expression);
        }
    }
}";

    }
}
