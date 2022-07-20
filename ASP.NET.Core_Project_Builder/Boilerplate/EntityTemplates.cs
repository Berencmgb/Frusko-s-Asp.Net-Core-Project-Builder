using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate;

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

    public const string ProgramTemplate =
@"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace {project}.Entity
{
    public static class Program
    {
        public static void Main(string[] args)
        {
        }
    }
    
    
    public class DbContextMigrationFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            builder.UseSqlServer(@""server=(localdb)\mssqllocaldb;database={project}Database;trusted_connection=yes;MultipleActiveResultSets=True"");
            return new AppDbContext(builder.Options);
        }
    }
}

";
}