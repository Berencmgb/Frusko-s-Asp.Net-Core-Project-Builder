namespace ASP.NET.Core_Project_Builder.Boilerplate
{
    public static class SharedTemplates
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
        public string? Reference { get; set; }
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
        public string? Message { get; set; }
        public T Value { get; set; }
    }

    public class Result
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
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

        public const string HttpPayloadTemplate =
@"namespace {namespace}
{
    public class HttpPayload
    {
        public string CurrentUserId { get; set; }
        public string SecurityToken { get; set; }
        public string Uri { get; set; }
    }
}";

        public const string PostBodyTemplate =
@"using {project}.Shared.Models;

namespace {namespace}
{
    public class PostBody
    {
        public string Expression { get; set; }
        public List<string> Includes { get; set; } = new List<string>();
        public Pagination Pagination { get; set; }
    }
}";

        public const string SharedDbContext =
@"using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using {project}.Shared.Models;

namespace {namespace}
{

    public class SharedDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public SharedDbContext(DbContextOptions options) : base(options)
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
using {project}.Shared.Models;
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
        public SharedDbContext Context;
        public DbSet<TEntity> Table;
        protected readonly IMapper _mapper;

        public List<string> Includes { get; set; } = new List<string>();
        protected ILogger<TRepository> _logger;

        public BaseRepository(SharedDbContext context, ILogger<TRepository> logger,
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
using {project}.Shared.Models;
using {project}.Shared.Helpers;

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

        public const string EnumHelpers =
@"using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace {namespace}
{
    public static class EnumHelpers
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            string displayName = """";
            displayName = enumValue.GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault()?
                .GetCustomAttribute<DisplayAttribute>()?
                .GetName();

            if (string.IsNullOrWhiteSpace(displayName))
                displayName = enumValue.ToString();

            return displayName;
        }   
    }
}";

        public const string BaseServiceClientTemplate =
@"using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using {project}.Shared.Models;
using {project}.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace {namespace}
{
    public class BaseServiceClient<TDto> : IBaseServiceClient<TDto> where TDto : BaseDTO
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public BaseServiceClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload)
        {
            var client = await GetClient(payload);

            var body = new PostBody { };

            var postbody = JsonConvert.SerializeObject(body);

            var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));

            var json = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }

        public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page)
        {
            var client = await GetClient(payload);
        
            var body = new PostBody { Pagination = page };
        
            var postbody = JsonConvert.SerializeObject(body);
        
            var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }
        
        public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, params Expression<Func<TDto, object>>[] includes)
        {
            var client = await GetClient(payload);
        
            var body = new PostBody { };
        
            foreach (var include in includes)
            {
                body.Includes.Add(include.ToString());
            }
        
            var postbody = JsonConvert.SerializeObject(body);
        
            var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }
        
        public async Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page, params Expression<Func<TDto, object>>[] includes)
        {
            var client = await GetClient(payload);
        
            var body = new PostBody { Pagination = page };
        
            foreach (var include in includes)
            {
                body.Includes.Add(include.ToString());
            }
        
            var postbody = JsonConvert.SerializeObject(body);
        
            var result = await client.PostAsync($""{AdaptiveUrl}/getallasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }
        
        public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes)
        {
            var client = await GetClient(payload);
        
            PostBody body = new() { Expression = expression.Simplify().ToString() };
        
            foreach (var include in includes)
            {
                body.Includes.Add(include.ToString());
            }
        
            var postbody = JsonConvert.SerializeObject(body);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
            var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }
        
        public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page, params Expression<Func<TDto, object>>[] includes)
        {
            var client = await GetClient(payload);
        
            PostBody body = new() { Expression = expression.Simplify().ToString(), Pagination = page };
        
            foreach (var include in includes)
            {
                body.Includes.Add(include.ToString());
            }
        
            var postbody = JsonConvert.SerializeObject(body);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
            var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }
        
        public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression)
        {
            var client = await GetClient(payload);
        
            PostBody body = new() { Expression = expression.Simplify().ToString() };
        
            var postbody = JsonConvert.SerializeObject(body);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
            var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }
        
        public async Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page)
        {
            var client = await GetClient(payload);
        
            PostBody body = new() { Expression = expression.Simplify().ToString(), Pagination = page };
        
            var postbody = JsonConvert.SerializeObject(body);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
            var result = await client.PostAsync($""{AdaptiveUrl}/getallwhereasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<List<TDto>>>(json);
        }
        
        public async Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes)
        {
            var client = await GetClient(payload);
        
            PostBody body = new() { Expression = expression.Simplify().ToString() };
        
            foreach (var include in includes)
            {
                body.Includes.Add(include.ToString());
            }
        
            var postbody = JsonConvert.SerializeObject(body);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
            var result = await client.PostAsync($""{AdaptiveUrl}/GetSingleWhereAsync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<TDto>(json);
        }
        
        public async Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression)
        {
            var client = await GetClient(payload);
        
            PostBody body = new() { Expression = expression.Simplify().ToString() };
        
            var postbody = JsonConvert.SerializeObject(body);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
            var result = await client.PostAsync($""{AdaptiveUrl}/GetSingleWhereAsync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<TDto>(json);
        }
        
        public async Task<Result<TDto>> CreateAsync(HttpPayload payload, TDto entityDTO)
        {
            try
            {
                var client = await GetClient(payload);
        
                var postbody = JsonConvert.SerializeObject(entityDTO);
        
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
                var result = await client.PostAsync($""{AdaptiveUrl}/createasync"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
                var json = await result.Content.ReadAsStringAsync();
        
                return JsonConvert.DeserializeObject<Result<TDto>>(json);
            }
            catch (Exception ex)
            {
                return new Result<TDto> { Success = false, Message = $""{ex.Message}"", Value = null };
            }
        
        }
        
        public async Task<Result<TDto>> UpdateAsync(HttpPayload payload, TDto entityDTO)
        {
            var client = await GetClient(payload);
        
            var postbody = JsonConvert.SerializeObject(entityDTO);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
            var result = await client.PutAsync($""{AdaptiveUrl}/Update"", new StringContent(postbody, Encoding.UTF8, ""application/json""));
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result<TDto>>(json);
        }
        
        public async Task<Result> DeleteAsync(HttpPayload payload, string reference)
        {
            var client = await GetClient(payload);
        
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(""application/json""));
        
            var result = await client.DeleteAsync($""{AdaptiveUrl}/Delete/{reference}"");
        
            var json = await result.Content.ReadAsStringAsync();
        
            return JsonConvert.DeserializeObject<Result>(json);
        }
        
        protected async Task<HttpClient> GetClient(HttpPayload payload)
        {
            var client = _httpClientFactory.CreateClient({project}WebConstants.ClientScope);
        
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, payload.SecurityToken);
        
            return client;
        }
        
        protected string AdaptiveUrl
        {
            get
            {
                return """";
            }
        }
    }

    public interface IBaseServiceClient<TDto> where TDto : BaseDTO
    {
        public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload);
    
        public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page);
    
        public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, params Expression<Func<TDto, object>>[] includes);
    
        public Task<Result<List<TDto>>> GetAllAsync(HttpPayload payload, Pagination page, params Expression<Func<TDto, object>>[] includes);
    
        public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes);
    
        public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page, params Expression<Func<TDto, object>>[] includes);
    
        public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression);
    
        public Task<Result<List<TDto>>> GetAllWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, Pagination page);
    
        public Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression, params Expression<Func<TDto, object>>[] includes);
    
        public Task<TDto> GetSingleWhereAsync(HttpPayload payload, Expression<Func<TDto, bool>> expression);
    
        public Task<Result<TDto>> CreateAsync(HttpPayload payload, TDto entityDTO);
    
        public Task<Result<TDto>> UpdateAsync(HttpPayload payload, TDto entityDTO);
    
        public Task<Result> DeleteAsync(HttpPayload payload, string reference);
    }
}";

        public const string WebConstantsTemplate =
@"using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace {namespace}
{
    public static class {project}WebConstants
    {
        public static string ClientScope { get => ""{project}-Web-Client""; }
        public static string SecurityKey { get; } = ""{new_id}"";

        public static SymmetricSecurityKey SymmetricSecurityKey { get; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));

        public static string LoginEndpoint { get => """"; }
        public static string RegisterEndpoint { get => """"; }

        public static JwtSecurityToken Token { get; set; }

        public static string LocalHostUrl { get => """"; }
        public static string HostUrl { get; set; } = LocalHostUrl;
        public static string LiveUrl { get; set; }
        public static string {project}WebIssuer { get => ""{project}-Web""; }
    }
}";

        public const string ApiConstantsTemplate =
@"using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace {namespace}
{
    public static class {project}ApiConstants
    {
        public static string Scope { get => ""{project}-Api""; }
        public static string SecurityKey { get; } = ""{new_id}"";
        public static SymmetricSecurityKey SymmetricSecurityKey { get; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
        public static string TestHostUrl { get; set; } = """";
        public static string HostUrl { get; set; } = TestHostUrl;
        public static string LiveUrl { get; set; } = """";
        public static string {project}ApiAudience { get => ""{project}-Api""; }
    }
}";

        public const string DynamicExpressionEvaluatorTemplate =
@"using System.Collections.Generic;
using System.Linq.Expressions;

namespace {namespace}
{
    public class DynamicExpressionEvaluator : ExpressionVisitor
    {
        private HashSet<Expression> dynamicExpressions;

        public DynamicExpressionEvaluator(HashSet<Expression> parameterlessExpressions)
        {
            this.dynamicExpressions = parameterlessExpressions;
        }
        public override Expression Visit(Expression node)
        {
            if (dynamicExpressions.Contains(node))
                return Evaluate(node);
            else
                return base.Visit(node);
        }

        private Expression Evaluate(Expression node)
        {
            if (node.NodeType == ExpressionType.Constant)
            {
                return node;
            }
            object value = Expression.Lambda(node).Compile().DynamicInvoke();
            return Expression.Constant(value, node.Type);
        }
    }
}";

        public const string DynamicExpressionSearcherTemplate =
@"using System.Collections.Generic;
using System.Linq.Expressions;

namespace {namespace}
{
    public class DynamicExpressionSearcher : ExpressionVisitor
    {
        public HashSet<Expression> DynamicExpressions { get; } = new HashSet<Expression>();
        private bool containsParameter = false;

        public override Expression Visit(Expression node)
        {
            bool originalContainsParameter = containsParameter;
            containsParameter = false;
            base.Visit(node);
            if (!containsParameter)
            {
                if (node?.NodeType == ExpressionType.Parameter)
                    containsParameter = true;
                else
                    DynamicExpressions.Add(node);
            }
            containsParameter |= originalContainsParameter;

            return node;
        }
    }
}";

        public const string TokenResolverTemplate =
@"using {project}.Shared.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace {namespace};

public class TokenResolver : ITokenResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public TokenResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> GetTokenAsync(string audience = null)
    {
        var signingCredentials = new SigningCredentials({project}ApiConstants.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: {project}WebConstants.{project}WebIssuer,
            audience: !string.IsNullOrWhiteSpace(audience) ? audience : {project}ApiConstants.{project}ApiAudience,
            signingCredentials: signingCredentials,
            claims: _httpContextAccessor.HttpContext?.User.Claims.ToList()
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GetTokenAsync(string audience = null, ClaimsPrincipal claims = null)
    {
        var signingCredentials = new SigningCredentials({project}ApiConstants.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: {project}WebConstants.{project}WebIssuer,
            audience: !string.IsNullOrWhiteSpace(audience) ? audience : {project}ApiConstants.{project}ApiAudience,
            signingCredentials: signingCredentials,
            claims: claims.Claims != null ? claims.Claims : _httpContextAccessor.HttpContext?.User.Claims.ToList()
        );
        

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public interface ITokenResolver
{
    public Task<string> GetTokenAsync(string audience = null);
    public Task<string> GetTokenAsync(string audience = null, ClaimsPrincipal claims = null);
}

";

        public const string CurrentUserTemplate =
@"namespace {namespace};

public class CurrentUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Username { get; set; }
    public string Reference { get; set; }
    public bool IsSignedIn { get; set; }
}";

        public const string IdentityResolverTemplate =
@"using {project}.Shared.Models;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace {namespace};

public class IdentityResolver : IIdentityResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdentityResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<CurrentUser> GetCurrentAccountAsync()
    {
        var currentUser = new CurrentUser();

        var firstName = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""FirstName"")?.Value;
        var lastName = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""LastName"")?.Value;
        var username = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""UserName"")?.Value;
        var email = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""Email"")?.Value;
        var reference = _httpContextAccessor.HttpContext.User?.Claims?.FirstOrDefault(c => c.Type == ""Reference"")?.Value;

        currentUser.FirstName = firstName;
        currentUser.LastName = lastName;
        currentUser.Username = username;
        currentUser.Email = email;
        currentUser.Reference = reference;
        currentUser.IsSignedIn = !string.IsNullOrWhiteSpace(currentUser.Reference);
        
        return currentUser;
    }
}

public interface IIdentityResolver
{
    public Task<CurrentUser> GetCurrentAccountAsync();
}";

        public const string BaseApiControllerTemplate =
@"using AutoMapper;
using {project}.Shared.Models;
using {project}.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace {project}.Shared.Utilities;

[Route(""[controller]"")]
public class BaseApiController<TEntity, TDto, TService, TController> : Controller where TEntity : class, IBaseEntity where TService : IBaseService<TEntity> where TDto : BaseDTO
{
    protected readonly TService Service;
    protected readonly IMapper _mapper;
    protected readonly ILogger<TController> _logger;

    public BaseApiController(TService service, IMapper mapper, ILogger<TController> logger)
    {
        Service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost(""GetAllAsync"")]
    //[Authorize(Policy = ""AccountOnly"")]
    public virtual async Task<IActionResult> GetAllAsync([FromBody] PostBody body)
    {
        var result = new Result<List<TEntity>> { };

        try
        {
            if (body.Includes.Count > 0 && body.Pagination != null)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllAsync(body.Pagination, includes);
            }
            else if (body.Includes.Count > 0)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllAsync(includes);
            }
            else if (body.Pagination != null)
                result = await Service.GetAllAsync(body.Pagination);

            else
                result = await Service.GetAllAsync();
        }
        catch(Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return Ok(_mapper.Map<Result<List<TDto>>>(result));
    }

    [HttpPost(""GetAllWhereAsync"")]
    public virtual async Task<IActionResult> GetAllWhereAsync([FromBody] PostBody body)
    {
        var result = new Result<List<TEntity>> { };

        try
        {
            if (!string.IsNullOrEmpty(body.Expression) && body.Includes.Count > 0 && body.Pagination != null)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllWhereAsync(body.Expression, includes, body.Pagination);
            }

            else if (!string.IsNullOrWhiteSpace(body.Expression) && body.Includes.Count > 0)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllWhereAsync(body.Expression, includes);
            }
            else if (body.Pagination != null)
                result = await Service.GetAllWhereAsync(body.Expression, body.Pagination);

            else
                result = await Service.GetAllWhereAsync(body.Expression);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return Ok(_mapper.Map<Result<List<TDto>>>(result));
    }

    [HttpPost(""GetSingleWhereAsync"")]
    public virtual async Task<IActionResult> GetSingleWhereAsync([FromBody] PostBody body)
    {
        TEntity entity = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(body.Expression) && body.Includes.Count > 0)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                entity = await Service.GetSingleWhereAsync(body.Expression, includes);
            }

            else
                entity = await Service.GetSingleWhereAsync(body.Expression);

            var dto = _mapper.Map<TDto>(entity);

            return Ok(dto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Ok(_mapper.Map<TDto>(entity));
        }
    }

    [HttpDelete(""Delete/{reference}"")]
    public virtual async Task<IActionResult> DeleteAsync(string reference)
    {
        var result = await Service.DeleteAsync(reference);

        return Ok(result);
    }

    [HttpPut(""Update"")]
    public virtual async Task<IActionResult> UpdateAsync([FromBody] TDto entityDTO)
    {
        var result = new Result<TEntity> { };

        try
        {
            var entity = await Service.GetSingleWhereAsync(e => e.Reference == entityDTO.Reference);

            _mapper.Map(entityDTO, entity);

            result = await Service.UpdateAsync(entity);
        }
        catch(Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }
        return Ok(_mapper.Map<Result<TDto>>(result));
    }

    [HttpPost(""CreateAsync"")]
    public virtual async Task<IActionResult> CreateAsync([FromBody] TDto entityDTO)
    {
        var result = new Result<TEntity> { };
        try
        {
            var entity = _mapper.Map<TEntity>(entityDTO);

            result = await Service.AddAsync(entity);

            var dto = _mapper.Map<Result<TDto>>(result);

            return Ok(dto);
        }
        catch(Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return Ok(_mapper.Map<Result<TDto>>(result));
    }
}

";

    }
}
