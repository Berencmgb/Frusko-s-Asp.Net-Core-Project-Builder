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
        public static string SolutionPrefix { get; set; } = "Epic Gamer Solution";
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
            var sharedNamespace = $"{SolutionPrefix}.Shared";

            Console.WriteLine("Generating Shared Project");

            var powerShell = PowerShell.Create();

            powerShell.Commands.AddScript($"dotnet new classlib --language c# -n {SolutionPrefix} -o {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace}");

            var output = powerShell.Invoke();

            if (output == null)
                return;

            foreach(var command in output)
            {
                Console.WriteLine(command.ToString());
            }

            powerShell.Commands.Clear();

            powerShell.Commands.AddScript($"dotnet sln {_absolutePath.Replace(" ", "` ")} add {_absolutePath.Replace(" ", "` ")}\\{sharedNamespace}");

            powerShell.Invoke();

            Console.WriteLine("Creating models folder");

            var modelsPath = @$"{sharedNamespace}\Models";
            var code = new StringBuilder();

            if (!Directory.Exists($"{_absolutePath}\\{modelsPath}"))
            {
                Directory.CreateDirectory($"{_absolutePath}\\{modelsPath}");
                Console.WriteLine($"Created shared models directory");
            }

            Console.WriteLine("Generating base entity");

            code.AppendLine($"namespace {modelsPath.Replace(@"\", ".")}");
            code.AppendLine("{");
            code.AppendLine($"    public class BaseEntity : IBaseEntity");
            code.AppendLine("    {");
            code.AppendLine("        public int Id { get; set; }");
            code.AppendLine("        public string Reference { get; set; }");
            code.AppendLine("        public bool IsDeleted { get; set; }");
            code.AppendLine("    }");
            code.AppendLine("");
            code.AppendLine($"    public interface IBaseEntity");
            code.AppendLine("    {");
            code.AppendLine("        public int Id { get; set; }");
            code.AppendLine("        public string Reference { get; set; }");
            code.AppendLine("        public bool IsDeleted { get; set; }");
            code.AppendLine("    }");
            code.AppendLine("}");

            await File.WriteAllTextAsync(@$"{_absolutePath}\\{modelsPath}\\BaseEntity.cs", code.ToString());

            


            // base entity and interface
            // base dto
            // result
            // base service client
            // base service
            // base repository


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
    }
}
