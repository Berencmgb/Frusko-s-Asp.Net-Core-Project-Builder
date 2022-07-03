using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate;
public static class ApiTemplates
{
    public const string ProgramTemplate =
@"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using {project}.Shared.Utilities;
using {project}.Shared.Models;
using {project}.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
    options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
});

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = new List<string>
            {
                {project}WebConstants.{project}WebIssuer
            },
            ValidAudiences = new List<string>
            {
                {project}ApiConstants.{project}ApiAudience
            },
            IssuerSigningKeys = new List<SecurityKey>
            {
                {project}WebConstants.SymmetricSecurityKey
            },
            RequireExpirationTime = false,
        };
    });

builder.Services.AddDbContext<AppDbContext>(c => c.UseSqlServer(builder.Configuration.GetConnectionString(""DefaultConnection""), o => o.EnableRetryOnFailure()));

builder.Services.AddScoped(typeof(IIdentityResolver), typeof(IdentityResolver));
builder.Services.AddScoped(typeof(ITokenResolver), typeof(TokenResolver));

// Add Repositories


// Add Services
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddUserManager<UserManager<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

";

    public const string AppSettingsTemplate =
@"{
  ""ConnectionStrings"": {
    ""DefaultConnection"": ""Server=(localdb)\\MSSQLLocalDB;Database={project}Database;Trusted_Connection=True""
  },
  ""Logging"": {
    ""LogLevel"": {
      ""Default"": ""Information"",
      ""Microsoft.AspNetCore"": ""Warning""
    }
  },
  ""AllowedHosts"": ""*""
}
";
}