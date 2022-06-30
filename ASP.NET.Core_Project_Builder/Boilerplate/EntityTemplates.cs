using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate
{
    public static class EntityTemplates
    {
        public const string AppDbContext =
@"using {project}.Shared.Utilities;
using Microsoft.EntityFrameworkCore;

namespace {namespace}
{
    public class AppDbContext : SharedDbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}";
    }
}
