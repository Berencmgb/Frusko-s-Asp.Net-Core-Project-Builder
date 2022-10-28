using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate;

public static class ServiceTemplates
{
    public const string AccountService =
@"using {project}.Shared.Utilities;
using {project}.Shared.Models;
using {project}.Entity;
using {project}.Repository;
using System;
using System.Threading.Tasks;

namespace {project}.Service;

public class AccountService : BaseService<User>, IAccountService
{
    public AccountService(IAccountRepository repository) : base(repository)
    {
    }
}

public interface IAccountService : IBaseService<User>
{

}

";

    public const string BaseServiceTemplate =
@"using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using {project}.Shared.Models;
using {project}.Shared.Helpers;
using {project}.Repository;

namespace {namespace};
public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : class, IBaseEntity
{
    public IBaseRepository<TEntity> Repository { get; set; }


    public BaseService(IBaseRepository<TEntity> repository)
    {
        Repository = repository;
    }

    #region GetAll

    public virtual async Task<Result<List<TEntity>>> GetAllAsync()
    {
        var result = await Repository.GetAllAsync();

        return result;
    }

    public virtual async Task<Result<List<TEntity>>> GetAllAsync(List<string> includes)
    {
        foreach (var include in includes)
        {
            Repository.Includes.Add(include);
        }
        return await Repository.GetAllAsync();
    }

    public virtual async Task<Result<List<TEntity>>> GetAllAsync(params Expression<Func<TEntity, object>>[] includes)
    {
        Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
        return await Repository.GetAllAsync();
    }

    public virtual async Task<Result<List<TEntity>>> GetAllAsync(Pagination page)
    {
        return await Repository.GetAllAsync(page);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllAsync(Pagination page, List<string> includes)
    {
        foreach (var include in includes)
        {
            Repository.Includes.Add(include);
        }
        return await Repository.GetAllAsync(page);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllAsync(Pagination page, params Expression<Func<TEntity, object>>[] includes)
    {
        Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
        return await Repository.GetAllAsync(page);
    }

    #endregion

    #region GetAllWhere

    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, List<string> includes)
    {
        foreach (var include in includes)
        {
            Repository.Includes.Add(include);
        }
        return await Repository.GetAllWhereAsync(expression);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
    {
        Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
        return await Repository.GetAllWhereAsync(expression);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await Repository.GetAllWhereAsync(expression);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression)
    {
        return await Repository.GetAllWhereAsync(expression);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination page, params Expression<Func<TEntity, object>>[] includes)
    {
        Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
        return await Repository.GetAllWhereAsync(expression, page);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(Expression<Func<TEntity, bool>> expression, Pagination page)
    {
        return await Repository.GetAllWhereAsync(expression, page);
    }

    public virtual async Task<Result<List<TEntity>>> GetAllWhereAsync(string expression, List<string> includes, Pagination page)
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

    public virtual async Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
    {
        Repository.Includes.AddRange(StringHelper.GenerateIncludes(includes));
        return await Repository.GetSingleWhereAsync(expression);
    }

    public virtual async Task<TEntity> GetSingleWhereAsync(string expression, List<string> includes)
    {
        foreach (var include in includes)
        {
            Repository.Includes.Add(include);
        }
        return await Repository.GetSingleWhereAsync(expression);
    }

    public virtual async Task<TEntity> GetSingleWhereAsync(Expression<Func<TEntity, bool>> expression)
    {
        return await Repository.GetSingleWhereAsync(expression);
    }

    public virtual async Task<TEntity> GetSingleWhereAsync(string expression)
    {
        return await Repository.GetSingleWhereAsync(expression);
    }

    #endregion

    public virtual async Task<Result> DeleteAsync(TEntity entity)
    {
        return await Repository.DeleteAsync(entity);
    }

    public virtual async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        return await Repository.InsertAsync(entity);
    }

    public virtual async Task<Result<TEntity>> UpdateAsync(TEntity entity)
    {
        return await Repository.UpdateAsync(entity);
    }

    public virtual async Task<Result> DeleteAsync(string reference)
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


}";
}