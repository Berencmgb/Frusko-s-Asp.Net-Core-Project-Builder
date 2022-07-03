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
builder.Services.AddScoped(typeof(IBaseServiceClient<>), typeof(BaseServiceClient<>));


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
    ]
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

exports.build = series(buildJs, buildCss, buildBundleJs, buildBundleCss);
exports.buildJs = buildJs;
exports.buildCss = buildCss;
exports.buildBundleJs = buildBundleJs;
exports.buildBundleCss = buildBundleCss;
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

namespace {namespace}.Controllers;

public class BaseController : Controller
{
    protected readonly IMapper _mapper;
    protected readonly ITokenResolver _tokenResolver;

    public BaseController(IMapper mapper, ITokenResolver tokenResolver)
    {
        _mapper = mapper;
        _tokenResolver = tokenResolver;
    }

    protected async Task<HttpPayload> GetPayload()
    {
        return new HttpPayload
        {
            SecurityToken = new JwtSecurityTokenHandler().WriteToken(await _tokenResolver.GetToken()),
            Uri = {project}ApiConstants.HostUrl
        };
    }
}
";

        public const string CreateUserViewModelTemplate =
@"using System.ComponentModel.DataAnnotations;

namespace {project}.ViewModels.Account;

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

        public const string AccountControllerTemplate =
@"using AutoMapper;
using {project}.Domain.Models;
using {project}.ServiceClients;
using {project}.Shared;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace {namespace}.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountServiceClient _accountServiceClient;
    private readonly IIdentityResolver _identityResolver;

    public AccountController(IMapper mapper,
        ITokenResolver tokenResolver,
        IAccountServiceClient accountServiceClient,
        IIdentityResolver identityResolver)
        : base(mapper,
            tokenResolver)
    {
        _accountServiceClient = accountServiceClient;
        _identityResolver = identityResolver;
    }

    [HttpGet(""Register"")]
    public async Task<IActionResult> Register()
    {
        var viewModel = new CreateUserViewModel {  };
        return View(viewModel);
    }

    [HttpPost(""Register"")]
    public async Task<IActionResult> Register(CreateUserViewModel viewModel)
    {
        var payload = await GetPayload();

        var userDTO = _mapper.Map<UserDTO>(viewModel);

        var result = await _accountServiceClient.RegisterAsync(payload, userDTO);
    }
    
}
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

    }

}