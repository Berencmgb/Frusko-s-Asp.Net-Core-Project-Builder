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
using {project}.Service;
using {project}.Repository;
using {project}.Entity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

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
builder.Services.AddScoped(typeof(IPayloadResolver), typeof(PayloadResolver));
builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));


// Add Repositories
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

// Add Services
builder.Services.AddScoped(typeof(IUserService), typeof(UserService));

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
using {project}.Service;
using {project}.Shared.Models;
using {project}.Domain.Models;
using {project}.Shared.Utilities;
using {project}.Shared.Helpers;
using {project}.Entity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace {project}.Api.Controllers;

[ApiController]
[Route(""[controller]"")]
public class UserController : BaseApiController<User, UserDTO, IUserService, UserController>
{
    private readonly ITokenResolver _tokenResolver;
    private readonly UserManager<User> _userManager;


    public UserController(IUserService service,
            IMapper mapper,
            ILogger<UserController> logger,
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
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO userDTO)
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
    public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
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
using {project}.Entity.Models;

namespace {project}.Api.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<User, UserDTO>().ReverseMap();

        CreateMap<RegisterUserDTO, User>();
        CreateMap<LoginUserDTO, User>();
    }
}

";

    public const string BaseApiControllerTemplate =
@"using AutoMapper;
using {project}.Shared.Models;
using {project}.Shared.Helpers;
using {project}.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace {project}.Shared.Utilities;

[Route(""[controller]"")]
public class BaseApiController<TEntity, TDto, TService, TController> : Controller where TEntity : class, IBaseEntity where TService : IBaseService<TEntity> where TDto : BaseDTO
{
    protected readonly TService Service;
    protected readonly IMapper _mapper;
    protected readonly ILogger<TController> _logger;

    public BaseApiController(TService service, IMapper mapper, ILogger<TController> logger)
    {
        Service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpPost(""GetAllAsync"")]
    //[Authorize(Policy = ""AccountOnly"")]
    public virtual async Task<IActionResult> GetAllAsync([FromBody] PostBody body)
    {
        var result = new Result<List<TEntity>> { };

        try
        {
            if (body.Includes.Count > 0 && body.Pagination != null)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllAsync(body.Pagination, includes);
            }
            else if (body.Includes.Count > 0)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllAsync(includes);
            }
            else if (body.Pagination != null)
                result = await Service.GetAllAsync(body.Pagination);

            else
                result = await Service.GetAllAsync();
        }
        catch(Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return Ok(_mapper.Map<Result<List<TDto>>>(result));
    }

    [HttpPost(""GetAllWhereAsync"")]
    public virtual async Task<IActionResult> GetAllWhereAsync([FromBody] PostBody body)
    {
        var result = new Result<List<TEntity>> { };

        try
        {
            if (!string.IsNullOrEmpty(body.Expression) && body.Includes.Count > 0 && body.Pagination != null)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllWhereAsync(body.Expression, includes, body.Pagination);
            }

            else if (!string.IsNullOrWhiteSpace(body.Expression) && body.Includes.Count > 0)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                result = await Service.GetAllWhereAsync(body.Expression, includes);
            }
            else if (body.Pagination != null)
                result = await Service.GetAllWhereAsync(body.Expression, body.Pagination);

            else
                result = await Service.GetAllWhereAsync(body.Expression);
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return Ok(_mapper.Map<Result<List<TDto>>>(result));
    }

    [HttpPost(""GetSingleWhereAsync"")]
    public virtual async Task<IActionResult> GetSingleWhereAsync([FromBody] PostBody body)
    {
        TEntity entity = null;
        try
        {
            if (!string.IsNullOrWhiteSpace(body.Expression) && body.Includes.Count > 0)
            {
                var includes = StringHelper.GenerateIncludes(body.Includes);

                entity = await Service.GetSingleWhereAsync(body.Expression, includes);
            }

            else
                entity = await Service.GetSingleWhereAsync(body.Expression);

            var dto = _mapper.Map<TDto>(entity);

            return Ok(dto);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Ok(_mapper.Map<TDto>(entity));
        }
    }

    [HttpDelete(""Delete/{reference}"")]
    public virtual async Task<IActionResult> DeleteAsync(string reference)
    {
        var result = await Service.DeleteAsync(reference);

        return Ok(result);
    }

    [HttpPut(""Update"")]
    public virtual async Task<IActionResult> UpdateAsync([FromBody] TDto entityDTO)
    {
        var result = new Result<TEntity> { };

        try
        {
            var entity = await Service.GetSingleWhereAsync(e => e.Reference == entityDTO.Reference);

            _mapper.Map(entityDTO, entity);

            result = await Service.UpdateAsync(entity);
        }
        catch(Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }
        return Ok(_mapper.Map<Result<TDto>>(result));
    }

    [HttpPost(""CreateAsync"")]
    public virtual async Task<IActionResult> CreateAsync([FromBody] TDto entityDTO)
    {
        var result = new Result<TEntity> { };
        try
        {
            var entity = _mapper.Map<TEntity>(entityDTO);

            result = await Service.AddAsync(entity);

            var dto = _mapper.Map<Result<TDto>>(result);

            return Ok(dto);
        }
        catch(Exception ex)
        {
            result.Success = false;
            result.Message = ex.Message;
        }

        return Ok(_mapper.Map<Result<TDto>>(result));
    }
}

";

    public const string UtilitiesMappingProfileTemplate =
@"using AutoMapper;
using {project}.Shared.Models;

namespace {project}.Api.MappingProfiles;

public class UtilitiesMappingProfile : Profile
{
	public UtilitiesMappingProfile()
	{
		CreateMap(typeof(Result<>), typeof(Result<>));
	}
}
";

}