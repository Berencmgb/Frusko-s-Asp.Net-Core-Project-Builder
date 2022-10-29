using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate;

public static class RepositoryTemplates
{
    public const string AccountRepositoryTemplate =
@"using AutoMapper;
using {project}.Entity;
using {project}.Shared.Models;
using {project}.Shared.Utilities;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace {project}.Repository;

public class AccountRepository : BaseRepository<User, IAccountRepository>, IAccountRepository
{
    public AccountRepository(AppDbContext context,
        ILogger<AccountRepository> logger,
        IMapper mapper)
        : base(context,
              logger,
              mapper)
    {
    }
}

public interface IAccountRepository : IBaseRepository<User>
{

}


";

    public const string BaseRepositoryTemplate =
@"using AutoMapper;
using {project}.Shared.Models;
using {project}.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.DynamicLinq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace {namespace};

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

    public virtual async Task<Result> DeleteAsync(TEntity entity)
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
    
    public virtual async Task<Result<List<TEntity>>> GetAllAsync()
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
        
    public virtual async Task<Result<List<TEntity>>> GetAllAsync(Pagination pagination)
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
        
    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression)
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
        
    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, Pagination pagination)
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
        
    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression)
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
        
    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination pagination)
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
        
    public virtual async Task<TEntity> GetSingleWhereAsync(string expression)
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
        
    public virtual async Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression)
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
        
    public virtual async Task<Result<TEntity>> UpdateAsync(TEntity entity)
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
        
    public virtual async Task<Result> DeleteAsync(string reference)
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
        
    public virtual async Task<Result> DeleteFromTableAsync(string reference)
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
";


    public const string UnitOfWorkTemplate =
@"using {project}.Entity;

namespace {project}.Repository;
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    public AppDbContext Context { get => _context; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await Context.SaveChangesAsync();
    }
}

public interface IUnitOfWork
{
    public AppDbContext Context { get; }
    Task SaveChangesAsync();
}

";

}