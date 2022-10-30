using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate;

public static class DomainTemplates
{
    public const string UserDTOTemplate =
@"using {project}.Shared.Models;

namespace {project}.Domain.Models;

public class UserDTO : BaseDTO
{
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
}
";
}
