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

public class AccountService : BaseService<User, IAccountRepository>, IAccountService
{
    public AccountService(IAccountRepository repository, IUnitOfWork unitOfWork) : base(repository, unitOfWork)
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

public class BaseService<TEntity, TRepository> : IBaseService<TEntity> where TEntity : class, IBaseEntity where TRepository : IBaseRepository<TEntity>
{
    private readonly IUnitOfWork _unitOfWork;

    public TRepository Repository { get; set; }
    public IUnitOfWork UnitOfWork { get => _unitOfWork; }

    public BaseService(TRepository repository, IUnitOfWork unitOfWork)
    {
        Repository = repository;
        _unitOfWork = unitOfWork;
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
        var result = await Repository.DeleteAsync(entity);

        await UnitOfWork.SaveChangesAsync();

        return result;
    }

    public virtual async Task<Result<TEntity>> AddAsync(TEntity entity)
    {
        var result = await Repository.InsertAsync(entity);

        await UnitOfWork.SaveChangesAsync();

        return result;
    }

    public virtual async Task<Result<TEntity>> UpdateAsync(TEntity entity)
    {
        var result = await Repository.UpdateAsync(entity);

        await UnitOfWork.SaveChangesAsync();

        return result;
    }

    public virtual async Task<Result> DeleteAsync(string reference)
    {
        var result = await Repository.DeleteAsync(reference);

        await UnitOfWork.SaveChangesAsync();

        return result;
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