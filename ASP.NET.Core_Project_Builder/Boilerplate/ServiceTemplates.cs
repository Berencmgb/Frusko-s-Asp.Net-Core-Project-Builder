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
using {project}.Repositories;
using System;
using System.Threading.Tasks;

namespace {project}.Services;

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
}