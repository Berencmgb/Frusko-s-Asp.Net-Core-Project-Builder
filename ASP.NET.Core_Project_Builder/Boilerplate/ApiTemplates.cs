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
using {project}.Services;
using {project}.Repositories;
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
builder.Services.AddScoped(typeof(IAccountRepository), typeof(AccountRepository));

// Add Services
builder.Services.AddScoped(typeof(IBaseService<>), typeof(BaseService<>));
builder.Services.AddScoped(typeof(IAccountService), typeof(AccountService));

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager<SignInManager<User>>()
    .AddUserManager<UserManager<User>>();

builder.Services.Configure<IdentityOptions>(o =>
{
    o.Password.RequiredUniqueChars = 0;
    o.Password.RequireNonAlphanumeric = false;
    o.Password.RequireDigit = false;
    o.Password.RequiredLength = 3;
    o.Password.RequireLowercase = false;
    o.Password.RequireUppercase = false;
});

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



    public const string AccountControllerTemplate =
@"using AutoMapper;
using {project}.Services;
using {project}.Shared.Models;
using {project}.Domain.Models;
using {project}.Shared.Utilities;
using {project}.Shared.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace {project}.Api.Controllers;

[Route(""[controller]"")]
[ApiController]
public class AccountController : BaseApiController<User, UserDTO, IAccountService, AccountController>
{
    private readonly ITokenResolver _tokenResolver;
    private readonly UserManager<User> _userManager;


    public AccountController(IAccountService service,
            IMapper mapper,
            ILogger<AccountController> logger,
            ITokenResolver tokenResolver,
            UserManager<User> userManager)
            : base(service,
                  mapper,
                  logger)
    {
        _tokenResolver = tokenResolver;
        _userManager = userManager;
    }


    [HttpPost(""Register"")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
        if(string.IsNullOrWhiteSpace(userDTO.Password) || string.IsNullOrWhiteSpace(userDTO.Password))
            return Ok(new Result<string> { Success = false, Message = ""No valid password submitted."" });
        
        if(userDTO.Password != userDTO.ConfirmPassword)
            return Ok(new Result<string> { Success = false, Message = ""Passwords do not match."" });
        
        var user = _mapper.Map<User>(userDTO);

        user.NormalizedEmail = userDTO.Email?.ToUpperInvariant();
        user.UserName = userDTO.Username?.ToUpperInvariant();

        var result = await _userManager.CreateAsync(user, userDTO.Password);
        
        if(!result.Succeeded)
            return Ok(new Result<string> { Success = false, Message = result?.Errors?.FirstOrDefault()?.Description ?? """" });
        
        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(""UserName"", user.UserName),
            new Claim(""Email"", user.Email),
            new Claim(""Reference"", user.Reference),
            new Claim(""FirstName"", user.FirstName),
            new Claim(""LastName"", user.LastName),
        }, ""{project} Cookie"");

        var token = await _tokenResolver.GetTokenAsync(claims: new ClaimsPrincipal(claimsIdentity));

        return Ok(new Result<string> { Success = true, Value = token });
    }

    [HttpPost(""Login"")]
    public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
    {
        var result = new Result<string> { };

        var user = await Service.GetSingleWhereAsync(a => a.NormalizedUserName == userDTO.Username.ToUpper());

        if (user == null)
            return Ok(new Result<string> { Success = false, Message = ""No user found."" });
        
        var canSignIn = await _userManager.CheckPasswordAsync(user, userDTO.Password);

        if(!canSignIn)
            return Ok(new Result<string> { Success = false, Message = ""Invalid Password."" });
        
        var claimsIdentity = new ClaimsIdentity(new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(""UserName"", user.UserName),
            new Claim(""Email"", user.Email),
            new Claim(""Reference"", user.Reference),
            new Claim(""FirstName"", user.FirstName),
            new Claim(""LastName"", user.LastName),
        }, ""{project} Cookie"");

        var token = await _tokenResolver.GetTokenAsync(claims: new ClaimsPrincipal(claimsIdentity));

        return Ok(new Result<string> { Success = true, Value = token });
    }

}
";

    public const string UserMappingProfileTemplate =
@"using AutoMapper;
using {project}.Shared.Models;
using {project}.Domain.Models;

namespace {project}.Api.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();
    }
}

";

}