using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder.Boilerplate
{
    public static class WebTemplates
    {

        public const string ProgramTemplate =
@"using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using {project}.Shared.Utilities;
using {project}.ServiceClient;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers().AddNewtonsoftJson(o => 
{
    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

// Service Client List
builder.Services.AddScoped(typeof(IIdentityResolver), typeof(IdentityResolver));
builder.Services.AddScoped(typeof(ITokenResolver), typeof(TokenResolver));
builder.Services.AddScoped(typeof(IPayloadResolver), typeof(PayloadResolver));
builder.Services.AddScoped(typeof(IBaseServiceClient<>), typeof(BaseServiceClient<>));
builder.Services.AddScoped(typeof(IUserServiceClient), typeof(UserServiceClient));

builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie(o =>
    {
        o.Cookie.Name = ""{project}.Cookie"";
        o.LoginPath = ""/Account/Login"";
    });

builder.Services.AddSession();

builder.Services.AddHttpClient({project}WebConstants.ClientScope, c =>
{
    c.BaseAddress = new Uri({project}ApiConstants.HostUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(""/Home/Error"");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: ""default"",
    pattern: ""{controller=Home}/{action=Index}/{id?}"");

app.Run();
";

        public const string GulpTemplate =
@"/// <binding BeforeBuild='build' />
'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass')(require('sass'));
var concat = require('gulp-concat');
const { series } = require('gulp');

var paths = {
    js: [
        ""./node_modules/jquery/dist/jquery.js"",
        ""./node_modules/jquery-validation/dist/jquery.validate.js"",
        ""./node_modules/jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js"",
        ""./node_modules/bootstrap/dist/js/bootstrap.bundle.js"",
    ],
    css: [
        ""./node_modules/bootstrap-icons/font/bootstrap-icons.css"",
        ""./node_modules/bootstrap/dist/css/bootstrap.css"",
    ],
    fonts: [
        `./node_modules/bootstrap-icons/font/fonts/bootstrap-icons.woff`,
    ], 
}

function buildJs(callback){
    return gulp.src(['./scripts/{project}.js', './scripts/*.js'])
        .pipe(concat('site.js'))
        .pipe(gulp.dest('./wwwroot/js'));
    callback();
}

function buildCss(callback){
    return gulp.src('./styles/Main.scss')
        .pipe(sass())
        .pipe(concat('site.css'))
        .pipe(gulp.dest('./wwwroot/css'));
    callback();
}

function buildBundleJs(callback){
    return gulp.src(paths.js)
        .pipe(concat('bundle.js'))
        .pipe(gulp.dest('wwwroot/js'));
    callback();
}

function buildBundleCss(callback){
    return gulp.src(paths.css)
        .pipe(sass())
        .pipe(concat('bundle.css'))
        .pipe(gulp.dest('./wwwroot/css'));
    callback();
}

function buildFonts(cb) {
    return gulp.src(paths.fonts)
        .pipe(gulp.dest('./wwwroot/css/fonts'));
    cb();
}

exports.build = series(buildJs, buildCss, buildBundleJs, buildBundleCss, buildFonts);
exports.buildJs = buildJs;
exports.buildCss = buildCss;
exports.buildBundleJs = buildBundleJs;
exports.buildBundleCss = buildBundleCss;
exports.buildFonts = buildFonts;
";

        public const string BaseControllerTemplate =
@"using AutoMapper;
using {project}.Domain.Models;
using {project}.Shared;
using {project}.Shared.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace {project}.Web.Controllers;

public class BaseController : Controller
{
    protected readonly IMapper _mapper;
    protected readonly ITokenResolver _tokenResolver;
    protected readonly IPayloadResolver _payloadResolver;

    public BaseController(IMapper mapper,
        ITokenResolver tokenResolver,
        IPayloadResolver payloadResolver)
    {
        _mapper = mapper;
        _tokenResolver = tokenResolver;
        _payloadResolver = payloadResolver;
    }
}
";

        public const string CreateUserViewModelTemplate =
@"using System.ComponentModel.DataAnnotations;

namespace {project}.Web.ViewModels.User;

public class CreateUserViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = ""Email"")]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare(""Password"", ErrorMessage = ""The password and confirmation password do not match."")]
    public string ConfirmPassword { get; set; }
}
";

        public const string UserViewModelTemplate =
@"namespace {project}.Web.ViewModels.User;

public class UserViewModel : BaseViewModel
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
";

        public const string UserMappingProfileTemplate =
@"using AutoMapper;
using {project}.Web.ViewModels.User;
using {project}.Domain.Models;

namespace {project}.Web.MappingProfiles;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<RegisterUserViewModel, RegisterUserDTO>().ReverseMap();
        CreateMap<LoginUserViewModel, LoginUserDTO>().ReverseMap();
        CreateMap<UserViewModel, UserDTO>().ReverseMap();
    }
}

";

        public const string AccountControllerTemplate =
@"using AutoMapper;
using {project}.Domain.Models;
using {project}.ServiceClient;
using {project}.Shared;
using {project}.Shared.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using {project}.Web.ViewModels.User;

namespace {project}.Web.Controllers;

public class UserController : BaseController
{
    private readonly IUserServiceClient _userServiceClient;
    private readonly IIdentityResolver _identityResolver;

    public UserController(IMapper mapper,
        ITokenResolver tokenResolver,
        IPayloadResolver payloadResolver,
        IUserServiceClient userServiceClient,
        IIdentityResolver identityResolver)
        : base(mapper,
            tokenResolver,
            payloadResolver)
    {
        _userServiceClient = userServiceClient;
        _identityResolver = identityResolver;
    }

    [HttpGet(""User/Register"")]
    public async Task<IActionResult> Register()
    {
        var viewModel = new RegisterUserViewModel {  };
        return View(viewModel);
    }

    [HttpPost(""User/Register"")]
    public async Task<IActionResult> Register(RegisterUserViewModel viewModel)
    {
        var payload = await _payloadResolver.GetPayloadAsync();

        var userDTO = _mapper.Map<RegisterUserDTO>(viewModel);

        var result = await _userServiceClient.RegisterAsync(payload, userDTO);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Value);

        var identity = new ClaimsPrincipal(new ClaimsIdentity(token.Claims, ""{project} Cookie""));

        await HttpContext.SignInAsync(identity);

        return RedirectToAction(""index"", ""home"");
    }

    [HttpGet(""User/Login"")]
    public async Task<IActionResult> Login()
    {
        var viewModel = new LoginUserViewModel {  };
        return View(viewModel);
    }

    [HttpPost(""User/Login"")]
    public async Task<IActionResult> Login(LoginUserViewModel viewModel)
    {
        var payload = await _payloadResolver.GetPayloadAsync();

        var userDTO = _mapper.Map<LoginUserDTO>(viewModel);

        var result = await _userServiceClient.LoginAsync(payload, userDTO);

        var token = new JwtSecurityTokenHandler().ReadJwtToken(result.Value);

        var identity = new ClaimsPrincipal(new ClaimsIdentity(token.Claims, ""{project} Cookie""));

        await HttpContext.SignInAsync(identity);

        return RedirectToAction(""index"", ""home"");
    }

    [HttpPost(""User/Logout"")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return RedirectToAction(""index"", ""home"");
    }    
}
";

        public const string RegisterHtmlTemplate =
@"@using {project}.Web.ViewModels.User

@model RegisterUserViewModel;


<form asp-action=""Register"" asp-controller=""User"">
    <input asp-for=""Email"" placeholder=""Email"" />
    <input asp-for=""FirstName"" placeholder=""FirstName"" />
    <input asp-for=""LastName"" placeholder=""LastName"" />
    <input asp-for=""Username"" placeholder=""Username"" />
    <input asp-for=""Password"" type=""password"" placeholder=""Password"" />
    <input asp-for=""ConfirmPassword"" type=""password"" placeholder=""Confirm Password"" />
    <button type=""submit"">Register</button>
</form>

";

        public const string BaseViewModelTemplate =
@"namespace {project}.Web.ViewModels;

public class BaseViewModel
{
    public string? Reference { get; set; }
    public bool IsDeleted { get; set; }
}
";

        public const string LoginHtmlTemplate =
@"@using {project}.Web.ViewModels.User

@model LoginUserViewModel;

<form asp-action=""Login"" asp-controller=""User"">
    <input asp-for=""Username"" placeholder=""Username"" />
    <input asp-for=""Password"" type=""password"" placeholder=""Password"" />
    <button type=""submit"">Login</button>
</form>

";


        public const string JqueryTemplate =
@"var {project_lower} = function (){
    this.name = '{project}';
}

$.{project_lower} = new {project_lower};
$.{project_lower}.Constructor = {project_lower};

{project_lower}.prototype.init = function(){
    console.log('jQuery inited');
}

";

        public const string LayoutTemplate =
@"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>@ViewData[""Title""] - {project}.Web</title>
    <link rel=""stylesheet"" href=""~/css/bundle.css"" asp-append-version=""true"" />
    <link rel=""stylesheet"" href=""~/css/site.css"" asp-append-version=""true"" />
</head>
<body>
    <header>
        <nav class=""navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow mb-3"">
            <div class=""container-fluid"">
                <a class=""navbar-brand"" asp-area="""" asp-controller=""Home"" asp-action=""Index"">{project}.Web</a>
                <button class=""navbar-toggler"" type=""button"" data-bs-toggle=""collapse"" data-bs-target="".navbar-collapse"" aria-controls=""navbarSupportedContent""
                        aria-expanded=""false"" aria-label=""Toggle navigation"">
                    <span class=""navbar-toggler-icon""></span>
                </button>
                <div class=""navbar-collapse collapse d-sm-inline-flex justify-content-between"">
                    <ul class=""navbar-nav flex-grow-1"">
                        <li class=""nav-item"">
                            <a class=""nav-link text-dark"" asp-area="""" asp-controller=""Home"" asp-action=""Index"">Home</a>
                        </li>
                        <li class=""nav-item"">
                            <a class=""nav-link text-dark"" asp-area="""" asp-controller=""Home"" asp-action=""Privacy"">Privacy</a>
                        </li>
                    </ul>
                </div>
            </div>
        </nav>
    </header>
    <div class=""container"">
        <main role=""main"" class=""pb-3"">
            @RenderBody()
        </main>
    </div>

    <footer class=""border-top footer text-muted"">
        <div class=""container"">
            &copy; 2022 - {project}.Web - <a asp-area="""" asp-controller=""Home"" asp-action=""Privacy"">Privacy</a>
        </div>
    </footer>
    <script src=""~/js/bundle.js"" asp-append-version=""true""></script>
    <script src=""~/js/site.js"" asp-append-version=""true""></script>
    <script>
        $.{project_lower}.init();
    </script>
    @await RenderSectionAsync(""Scripts"", required: false)
</body>
</html>
";

        public const string SiteCssTemplate =
@"html {
  font-size: 14px;
}

@media (min-width: 768px) {
  html {
    font-size: 16px;
  }
}

html {
  position: relative;
  min-height: 100%;
}

body {
  margin-bottom: 60px;
}
";

        public const string RegisterUserViewModelTemplate =
@"using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace {project}.Web.ViewModels.User;
public class RegisterUserViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = ""Email"")]
    public string Email { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare(""Password"", ErrorMessage = ""The password and confirmation password do not match."")]
    public string ConfirmPassword { get; set; }
}";

        public const string LoginUserViewModelTemplate =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace {project}.Web.ViewModels.User;
public class LoginUserViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}";

    }

}