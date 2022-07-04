using ASP.NET.Core_Project_Builder.Boilerplate;
using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder
{
    public static class CodeGenerator
    {
        public static string SolutionPrefix { get; set; } = "Blink2";
        public static string SolutionPath { get; set; } = @"C:\Users\Beren\Documents\Projects\Test Projects";
        private static string _absolutePath { get => $"{SolutionPath}\\{SolutionPrefix}"; }
        private static string _entityNamespace { get => $"{SolutionPrefix}.Entity"; }
        private static string _domainNamespace { get => $"{SolutionPrefix}.Domain"; }
        private static string _serviceClientNamespace { get => $"{SolutionPrefix}.ServiceClient"; }
        private static string _serviceNamespace { get => $"{SolutionPrefix}.Service"; }
        private static string _repositoryNamespace { get => $"{SolutionPrefix}.Repository"; }
        private static string _webNamespace { get => $"{SolutionPrefix}.Web"; }
        private static string _apiNamespace { get => $"{SolutionPrefix}.Api"; }
        private static string _sharedNamespace { get => $"{SolutionPrefix}.Shared"; }


        /// <summary>
        /// Generates the project's root folder that contains the solution.
        /// </summary>
        /// <returns></returns>
        public static async Task GenerateRootFolder()
        {
            SolutionPrefix = SolutionPrefix.Replace(" ", "_");

            Console.WriteLine("Checking file path");

            if (!Directory.Exists(_absolutePath))
            {
                Directory.CreateDirectory(_absolutePath);
                Console.WriteLine($"Directory Created: {_absolutePath}");
            }

            var powerShell = PowerShell.Create();

            Console.WriteLine("Creating solution");

            powerShell.Commands.AddScript($"dotnet new sln -o {_absolutePath.Replace(" ", "` ")}");

            var output = powerShell.Invoke();

            if (output == null)
                return;

            foreach (var command in output)
            {
                Console.WriteLine(command.ToString());
            }
        }

        public static async Task GenerateSharedProject()
        {
            var powerShell = PowerShell.Create();
            var sharedNamespace = $"{SolutionPrefix}.Shared";

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {sharedNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace}");
            Console.WriteLine("STEP: Generating Shared Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace}");
            Console.WriteLine("STEP: Adding Shared Project to Solution");
            await InvokePowershell(powerShell);

            Console.WriteLine("Removing Class1.cs");

            if (File.Exists($"{_absolutePath}\\{sharedNamespace}\\Class1.cs"))
                File.Delete($"{_absolutePath}\\{sharedNamespace}\\Class1.cs");

            #endregion

            #region Installing Nuget Packages

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package AutoMapper");
            Console.WriteLine("STEP: Adding Automapper");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Microsoft.Extensions.Http");
            Console.WriteLine("STEP: Adding Microsoft Extensions Http");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Microsoft.AspNetCore.Authentication.JwtBearer");
            Console.WriteLine("STEP: Adding Microsoft AspNetCore Authentication JwtBearer");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Microsoft.EntityFrameworkCore");
            Console.WriteLine("STEP: Adding Entity Framework Core");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Newtonsoft.Json");
            Console.WriteLine("STEP: Adding Newtonsoft Json");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Microsoft.Extensions.Logging");
            Console.WriteLine("STEP: Adding Logging");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Microsoft.EntityFrameworkCore.DynamicLinq");
            Console.WriteLine("STEP: Adding Dynamic Linq");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Microsoft.AspNetCore.Identity");
            Console.WriteLine("STEP: Adding Microsoft Identity");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace} package Microsoft.AspNetCore.Identity.EntityFrameworkCore");
            Console.WriteLine("STEP: Adding Microsoft Identity Entity Framework Core");
            await InvokePowershell(powerShell);


            powerShell.Commands.AddScript($"dotnet restore {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace}");
            Console.WriteLine("STEP: Restoring Nuget Packages");
            await InvokePowershell(powerShell);

            #endregion

            #region Generating Models

            Console.WriteLine("STEP: Creating models folder");

            var modelsPath = @$"{sharedNamespace}\Models";

            if (!Directory.Exists($"{_absolutePath}\\{modelsPath}"))
            {
                Directory.CreateDirectory($"{_absolutePath}\\{modelsPath}");
                Console.WriteLine($"Created shared models directory");
            }
            var modelNamespace = modelsPath.Replace(@"\", ".");

            Console.WriteLine("STEP: Generating base entity");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\BaseEntity.cs", SharedTemplates.BaseEntityTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating base dto");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\BaseDTO.cs", SharedTemplates.BaseDTOTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating result");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\Result.cs", SharedTemplates.ResultTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating pagination");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\Pagination.cs", SharedTemplates.PaginationTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating user");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\User.cs", SharedTemplates.UserTemplate.Replace("{namespace}", modelNamespace));
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\CurrentUser.cs", SharedTemplates.CurrentUserTemplate.Replace("{namespace}", modelNamespace));

            #endregion

            #region Generating Utilities

            var utiltiesPath = @$"{sharedNamespace}\Utilities";

            if (!Directory.Exists($"{_absolutePath}\\{utiltiesPath}"))
            {
                Directory.CreateDirectory($"{_absolutePath}\\{utiltiesPath}");
                Console.WriteLine($"Created shared models directory");
            }

            var utilitiesNamespace = utiltiesPath.Replace(@"\", ".");

            var securityKey = Guid.NewGuid().ToString();

            Console.WriteLine("STEP: Generating shared db context");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\SharedDbContext.cs", SharedTemplates.SharedDbContext.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating base repository");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\BaseRepository.cs", SharedTemplates.BaseRepositoryTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating base service");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\BaseService.cs", SharedTemplates.BaseServiceTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating base service client");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\BaseServiceClient.cs", SharedTemplates.BaseServiceClientTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating http payload");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\HttpPayload.cs", SharedTemplates.HttpPayloadTemplate.Replace("{namespace}", utilitiesNamespace));

            Console.WriteLine("STEP: Generating postbody");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\PostBody.cs", SharedTemplates.PostBodyTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating token resolver");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\TokenResolver.cs", SharedTemplates.TokenResolverTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating identity resolver");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\IdentityResolver.cs", SharedTemplates.IdentityResolverTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating web constants");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\{SolutionPrefix}WebConstants.cs", SharedTemplates.WebConstantsTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix).Replace("{new_id}", securityKey));

            Console.WriteLine("STEP: Generating api constants");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\{SolutionPrefix}ApiConstants.cs", SharedTemplates.ApiConstantsTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix).Replace("{new_id}", securityKey));

            #endregion

            #region Generating Helpers

            var helpersPath = @$"{sharedNamespace}\Helpers";

            if (!Directory.Exists($"{_absolutePath}\\{helpersPath}"))
            {
                Directory.CreateDirectory($"{_absolutePath}\\{helpersPath}");
                Console.WriteLine($"Created shared models directory");
            }

            var helpersNamespace = helpersPath.Replace(@"\", "."); 

            Console.WriteLine("STEP: Generating string helpers");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\StringHelpers.cs", SharedTemplates.StringHelpersTemplate.Replace("{namespace}", helpersNamespace));

            Console.WriteLine("STEP: Generating expression helpers");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\ExpressionHelpers.cs", SharedTemplates.ExpressionHelpersTemplate.Replace("{namespace}", helpersNamespace));

            Console.WriteLine("STEP: Generating expression evaluator");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\DynamicExpressionEvaluator.cs", SharedTemplates.DynamicExpressionEvaluatorTemplate.Replace("{namespace}", helpersNamespace));

            Console.WriteLine("STEP: Generating expression searcher");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\DynamicExpressionSearcher.cs", SharedTemplates.DynamicExpressionSearcherTemplate.Replace("{namespace}", helpersNamespace));

            Console.WriteLine("STEP: Generating enum helpers");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\EnumHelpers.cs", SharedTemplates.EnumHelpers.Replace("{namespace}", helpersNamespace));

            #endregion


            return;
        }

        public static async Task GenerateEntityProject()
        {
            var powerShell = PowerShell.Create();
            var entityNamespace = $"{SolutionPrefix}.Entity";

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {entityNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{entityNamespace}");
            Console.WriteLine("STEP: Generating Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{entityNamespace}");
            Console.WriteLine("STEP: Adding Entity Project to Solution");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_entityNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_sharedNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            Console.WriteLine("Removing Class1.cs");

            if (File.Exists($"{_absolutePath}\\{entityNamespace}\\Class1.cs"))
                File.Delete($"{_absolutePath}\\{entityNamespace}\\Class1.cs");

            #endregion

            #region Install Nuget Packages

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{entityNamespace} package Microsoft.EntityFrameworkCore");
            Console.WriteLine("STEP: Adding Entity Framework Core");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{entityNamespace} package Microsoft.EntityFrameworkCore.SqlServer");
            Console.WriteLine("STEP: Adding Entity Framework Core Sql Server");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{entityNamespace} package Microsoft.EntityFrameworkCore.Tools");
            Console.WriteLine("STEP: Adding Entity Framework Core Tools");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{entityNamespace} package Microsoft.EntityFrameworkCore.Design");
            Console.WriteLine("STEP: Adding Entity Framework Core Design");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{entityNamespace} package Microsoft.AspNetCore.Identity.EntityFrameworkCore");
            Console.WriteLine("STEP: Adding Entity Framework Core Identity");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{entityNamespace} package Microsoft.Diagnostics.EntityFrameworkCore");
            Console.WriteLine("STEP: Adding Entity Framework Core Diagnostics");
            await InvokePowershell(powerShell);

            #endregion

            Console.WriteLine("STEP: Creating models folder");

            var modelsPath = @$"{entityNamespace}\Models";

            if (!Directory.Exists($"{_absolutePath}\\{modelsPath}"))
            {
                Directory.CreateDirectory($"{_absolutePath}\\{modelsPath}");
                Console.WriteLine($"Created entity models directory");
            }

            Console.WriteLine("STEP: Generating app db context");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{_entityNamespace}\\AppDbContext.cs", EntityTemplates.AppDbContext.Replace("{namespace}", entityNamespace).Replace("{project}", SolutionPrefix));

            return;
        }

        public static async Task GenerateDomainProject()
        {
            var powerShell = PowerShell.Create();

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {_domainNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{_domainNamespace}");
            Console.WriteLine("STEP: Generating Domain Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{_domainNamespace}");
            Console.WriteLine("STEP: Adding Entity Project to Solution");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_domainNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_sharedNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            Console.WriteLine("Removing Class1.cs");

            if (File.Exists($"{_absolutePath}\\{_domainNamespace}\\Class1.cs"))
                File.Delete($"{_absolutePath}\\{_domainNamespace}\\Class1.cs");

            #endregion

            Console.WriteLine("STEP: Creating models folder");

            var modelsPath = @$"{_domainNamespace}\Models";

            if (!Directory.Exists($"{_absolutePath}\\{modelsPath}"))
            {
                Directory.CreateDirectory($"{_absolutePath}\\{modelsPath}");
                Console.WriteLine($"Created domain models directory");
            }

            File.WriteAllText($"{_absolutePath}\\{_domainNamespace}\\Models\\UserDTO.cs", DomainTemplates.UserDTOTemplate.Replace("{project}", SolutionPrefix));

            return;
        }

        public static async Task GenerateServiceClientProject()
        {
            var powerShell = PowerShell.Create();

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {_serviceClientNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{_serviceClientNamespace}");
            Console.WriteLine("STEP: Generating Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{_serviceClientNamespace}");
            Console.WriteLine("STEP: Adding Entity Project to Solution");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_serviceClientNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_sharedNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_serviceClientNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_domainNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            Console.WriteLine("Removing Class1.cs");

            if (File.Exists($"{_absolutePath}\\{_serviceClientNamespace}\\Class1.cs"))
                File.Delete($"{_absolutePath}\\{_serviceClientNamespace}\\Class1.cs");

            #endregion

            #region Install Nuget Packages

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_serviceClientNamespace} package Microsoft.AspNetCore.Authentication.JwtBearer");
            Console.WriteLine("STEP: Adding Microsoft AspNetCore Authentication JwtBearer");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_serviceClientNamespace} package Newtonsoft.Json");
            Console.WriteLine("STEP: Adding Newtonsoft Json");
            await InvokePowershell(powerShell);

            #endregion

            File.WriteAllText($"{_absolutePath}\\{_serviceClientNamespace}\\AccountServiceClient.cs", ServiceClientTemplates.AccountServiceClientTemplate.Replace("{project}", SolutionPrefix));

            return;
        }

        public static async Task GenerateRepositoryProject()
        {
            var powerShell = PowerShell.Create();

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {_repositoryNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace}");
            Console.WriteLine("STEP: Generating Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace}");
            Console.WriteLine("STEP: Adding Entity Project to Solution");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_sharedNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_entityNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            Console.WriteLine("Removing Class1.cs");

            if (File.Exists($"{_absolutePath}\\{_repositoryNamespace}\\Class1.cs"))
                File.Delete($"{_absolutePath}\\{_repositoryNamespace}\\Class1.cs");

            #endregion

            #region Install Nuget Packages

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace} package Automapper");
            Console.WriteLine("STEP: Adding Automapper");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace} package Microsoft.EntityFrameworkCore.DynamicLinq");
            Console.WriteLine("STEP: Adding Microsoft EntityFrameworkCore DynamicLinq");
            await InvokePowershell(powerShell);

            #endregion
        }

        public static async Task GenerateServiceProject()
        {
            var powerShell = PowerShell.Create();

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {_serviceNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{_serviceNamespace}");
            Console.WriteLine("STEP: Generating Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{_serviceNamespace}");
            Console.WriteLine("STEP: Adding Entity Project to Solution");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_serviceNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_sharedNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_serviceNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_entityNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_serviceNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Entity Project");
            await InvokePowershell(powerShell);

            Console.WriteLine("Removing Class1.cs");

            if (File.Exists($"{_absolutePath}\\{_serviceNamespace}\\Class1.cs"))
                File.Delete($"{_absolutePath}\\{_serviceNamespace}\\Class1.cs");

            #endregion
        }

        public static async Task GenerateWebProject()
        {
            var powerShell = PowerShell.Create();

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new mvc --language c# -n {_webNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{_webNamespace}");
            Console.WriteLine("STEP: Generating Web Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace}");
            Console.WriteLine("STEP: Adding Web Project to Solution");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_sharedNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Web Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_domainNamespace}");
            Console.WriteLine("STEP: Linking Domain Project to Web Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_serviceClientNamespace}");
            Console.WriteLine("STEP: Linking Service Client Project to Web Project");
            await InvokePowershell(powerShell);

            #endregion

            #region Install Nuget Packages

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} package Automapper");
            Console.WriteLine("STEP: Adding Automapper");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} package AutoMapper.Extensions.Microsoft.DependencyInjection");
            Console.WriteLine("STEP: Adding Automapper Extensions Microsoft DependancyInjection");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} package Newtonsoft.Json");
            Console.WriteLine("STEP: Adding Newtonsoft Json");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} package Microsoft.AspNetCore.Mvc.NewtonsoftJson");
            Console.WriteLine("STEP: Adding Microsoft AspNetCore Mvc NewtonsoftJson");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} package Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation");
            Console.WriteLine("STEP: Adding Microsoft AspNetCore Razor RuntimeCompilation");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_webNamespace} package Microsoft.EntityFrameworkCore.Design");
            Console.WriteLine("STEP: Adding Microsoft EntityFrameworkCore Design");
            await InvokePowershell(powerShell);

            #endregion

            #region Install Node Packages

            powerShell.Commands.AddScript($"cd {_absolutePath.Replace(" ", "` ")}\\{_webNamespace}");
            powerShell.Commands.AddScript($"npm init -y");
            powerShell.Commands.AddScript($"npm install jquery --save-dev");
            powerShell.Commands.AddScript($"npm install jquery-validation --save-dev");
            powerShell.Commands.AddScript($"npm install jquery-validation-unobtrusive --save-dev");
            powerShell.Commands.AddScript($"npm install gulp --save-dev");
            powerShell.Commands.AddScript($"npm install gulp-sass --save-dev");
            powerShell.Commands.AddScript($"npm install gulp-concat --save-dev");
            powerShell.Commands.AddScript($"npm install sass --save-dev");
            powerShell.Commands.AddScript($"npm install bootstrap");
            powerShell.Commands.AddScript($"npm install bootstrap-icons");
            Console.WriteLine("STEP: Adding Node Packages");
            await InvokePowershell(powerShell);

            #endregion

            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\Program.cs", WebTemplates.ProgramTemplate.Replace("{project}", SolutionPrefix));
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\Gulpfile.js", WebTemplates.GulpTemplate.Replace("{project}", SolutionPrefix));

            if (!Directory.Exists($"{_absolutePath}\\{_webNamespace}\\Scripts"))
                Directory.CreateDirectory($"{_absolutePath}\\{_webNamespace}\\Scripts");

            if (!Directory.Exists($"{_absolutePath}\\{_webNamespace}\\Styles"))
                Directory.CreateDirectory($"{_absolutePath}\\{_webNamespace}\\Styles");

            if (!Directory.Exists($"{_absolutePath}\\{_webNamespace}\\ViewModels"))
                Directory.CreateDirectory($"{_absolutePath}\\{_webNamespace}\\ViewModels");

            if (!Directory.Exists($"{_absolutePath}\\{_webNamespace}\\ViewModels\\Account"))
                Directory.CreateDirectory($"{_absolutePath}\\{_webNamespace}\\ViewModels\\Account");

            if (!Directory.Exists($"{_absolutePath}\\{_webNamespace}\\MappingProfiles"))
                Directory.CreateDirectory($"{_absolutePath}\\{_webNamespace}\\MappingProfiles");

            if (!Directory.Exists($"{_absolutePath}\\{_webNamespace}\\Controllers"))
                Directory.CreateDirectory($"{_absolutePath}\\{_webNamespace}\\Controllers");

            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\Scripts\\{SolutionPrefix}.js", WebTemplates.JqueryTemplate.Replace("{project}", SolutionPrefix).Replace("{project_lower}", SolutionPrefix.ToLower()));
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\Styles\\Main.scss", WebTemplates.SiteCssTemplate);
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\Views\\Shared\\_Layout.cshtml", WebTemplates.LayoutTemplate.Replace("{project}", SolutionPrefix).Replace("{project_lower}", SolutionPrefix.ToLower()));
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\ViewModels\\Account\\CreateUserViewModel.cs", WebTemplates.CreateUserViewModelTemplate.Replace("{project}", SolutionPrefix));
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\ViewModels\\Account\\UserViewModel.cs", WebTemplates.UserViewModelTemplate.Replace("{project}", SolutionPrefix));
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\MappingProfiles\\UserMappingProfile.cs", WebTemplates.UserMappingProfileTemplate.Replace("{project}", SolutionPrefix));
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\Controllers\\BaseController.cs", WebTemplates.BaseControllerTemplate.Replace("{project}", SolutionPrefix));
            File.WriteAllText($"{_absolutePath}\\{_webNamespace}\\Controllers\\AccountController.cs", WebTemplates.AccountControllerTemplate.Replace("{project}", SolutionPrefix));

            return;
        }

        public static async Task GenerateAPI()
        {
            var powerShell = PowerShell.Create();

            #region Generating Project

            powerShell.Commands.AddScript($"dotnet new 	webapi --language c# -n {_apiNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace}");
            Console.WriteLine("STEP: Generating Api Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace}");
            Console.WriteLine("STEP: Adding Api Project to Solution");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_sharedNamespace}");
            Console.WriteLine("STEP: Linking Shared Project to Api Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_domainNamespace}");
            Console.WriteLine("STEP: Linking Domain Project to Api Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_serviceNamespace}");
            Console.WriteLine("STEP: Linking Service Project to Api Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_repositoryNamespace}");
            Console.WriteLine("STEP: Linking Repository Project to Api Project");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} reference {_absolutePath.Replace(" ", "` ")}\\{_entityNamespace}");
            Console.WriteLine("STEP: Linking Entity Project to Api Project");
            await InvokePowershell(powerShell);

            #endregion

            #region Install Nuget Packages

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package Automapper");
            Console.WriteLine("STEP: Adding Automapper");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package AutoMapper.Extensions.Microsoft.DependencyInjection");
            Console.WriteLine("STEP: Adding Automapper Extensions Microsoft DependancyInjection");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package Microsoft.AspNetCore.Authentication.JwtBearer");
            Console.WriteLine("STEP: Adding Microsoft AspNetCore Authentication JwtBearer");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package Microsoft.AspNetCore.Mvc.NewtonsoftJson");
            Console.WriteLine("STEP: Adding Microsoft AspNetCore Mvc NewtonsoftJson");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package Newtonsoft.Json");
            Console.WriteLine("STEP: Adding Newtonsoft Json");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package Microsoft.EntityFrameworkCore");
            Console.WriteLine("STEP: Adding Microsoft EntityFrameworkCore");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package Microsoft.EntityFrameworkCore.SqlServer");
            Console.WriteLine("STEP: Adding Microsoft EntityFrameworkCore SqlServer");
            await InvokePowershell(powerShell);

            powerShell.Commands.AddScript($"dotnet add {_absolutePath.Replace(" ", "` ")}\\{_apiNamespace} package Microsoft.EntityFrameworkCore.Design");
            Console.WriteLine("STEP: Adding Microsoft EntityFrameworkCore Design");
            await InvokePowershell(powerShell);


            #endregion

            File.WriteAllText($"{_absolutePath}\\{_apiNamespace}\\Program.cs", ApiTemplates.ProgramTemplate.Replace("{project}", SolutionPrefix));
            File.WriteAllText($"{_absolutePath}\\{_apiNamespace}\\AppSettings.json", ApiTemplates.AppSettingsTemplate.Replace("{project}", SolutionPrefix));

            return;
        }


        private static async Task InvokePowershell(PowerShell powerShell)
        {
            var output = powerShell.Invoke();

            if (output == null)
                return;

            foreach (var command in output)
            {
                Console.WriteLine(command.ToString());
            }

            powerShell.Commands.Clear();
        }
    }
}
