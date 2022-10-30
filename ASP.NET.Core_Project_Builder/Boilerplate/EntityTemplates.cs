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
using {project}.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using {project}.Entity.Models;

namespace {project}.Entity;

public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
{
    public AppDbContext(DbContextOptions options) : base(options)
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
}";

    public const string ProgramTemplate =
@"using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace {project}.Entity;

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
";

    public const string UserTemplate =
@"using Microsoft.AspNetCore.Identity;
using {project}.Shared.Models;

namespace {project}.Entity.Models;

public class User : IdentityUser<int>, IBaseEntity
{
    public string Reference { get; set; }
    public bool IsDeleted { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
";
}