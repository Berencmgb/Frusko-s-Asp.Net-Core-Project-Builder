﻿using Microsoft.Build.Construction;
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
        public static string SolutionPrefix { get; set; } = "Blink";
        public static string SolutionPath { get; set; } = @"C:\Users\Beren\Documents\Projects\Test Projects";

        private static string _absolutePath { get => $"{SolutionPath}\\{SolutionPrefix}"; }


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

            #region Installing Nuget Packages

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {sharedNamespace} -o {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace}");
            Console.WriteLine("STEP: Generating Shared Project");
            await InvokePowershell(powerShell);


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


            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace}");
            Console.WriteLine("STEP: Adding Shared Project to Solution");
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
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\BaseEntity.cs", Boilerplate.BaseEntityTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating base dto");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\BaseDTO.cs", Boilerplate.BaseDTOTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating result");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\Result.cs", Boilerplate.ResultTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating pagination");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\Pagination.cs", Boilerplate.PaginationTemplate.Replace("{namespace}", modelNamespace));

            Console.WriteLine("STEP: Generating user");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\User.cs", Boilerplate.UserTemplate.Replace("{namespace}", modelNamespace));

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
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\SharedDBContext.cs", Boilerplate.SharedDBContext.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating base repository");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\BaseRepository.cs", Boilerplate.BaseRepositoryTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating base service");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\BaseService.cs", Boilerplate.BaseServiceTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating base service client");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\BaseServiceClient.cs", Boilerplate.BaseServiceClientTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating http payload");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\HttpPayload.cs", Boilerplate.HttpPayloadTemplate.Replace("{namespace}", utilitiesNamespace));

            Console.WriteLine("STEP: Generating postbody");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\PostBody.cs", Boilerplate.PostBodyTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix));

            Console.WriteLine("STEP: Generating web constants");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\{SolutionPrefix}WebConstants.cs", Boilerplate.WebConstantsTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix).Replace("{new_id}", securityKey));

            Console.WriteLine("STEP: Generating api constants");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{utiltiesPath}\\{SolutionPrefix}ApiConstants.cs", Boilerplate.ApiConstantsTemplate.Replace("{namespace}", utilitiesNamespace).Replace("{project}", SolutionPrefix).Replace("{new_id}", securityKey));

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
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\StringHelpers.cs", Boilerplate.StringHelpersTemplate.Replace("{namespace}", helpersNamespace));

            Console.WriteLine("STEP: Generating string helpers");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\ExpressionHelpers.cs", Boilerplate.ExpressionHelpersTemplate.Replace("{namespace}", helpersNamespace));

            Console.WriteLine("STEP: Generating expression evaluator");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\DynamicExpressionEvaluator.cs", Boilerplate.DynamicExpressionEvaluatorTemplate.Replace("{namespace}", helpersNamespace));

            Console.WriteLine("STEP: Generating expression searcher");
            await File.WriteAllTextAsync(@$"{_absolutePath}\\{helpersPath}\\DynamicExpressionSearcher.cs", Boilerplate.DynamicExpressionSearcherTemplate.Replace("{namespace}", helpersNamespace));

            #endregion


            return;
        }



        public static async Task GenerateWebProject()
        {
            return;
        }

        public static async Task GenerateAPI()
        {
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
