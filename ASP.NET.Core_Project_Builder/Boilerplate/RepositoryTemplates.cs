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

namespace {project}.Repositories;

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

}