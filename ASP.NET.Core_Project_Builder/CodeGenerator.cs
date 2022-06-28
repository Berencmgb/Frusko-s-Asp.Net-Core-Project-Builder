using Microsoft.Build.Construction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET.Core_Project_Builder
{
    public static class CodeGenerator
    {
        public static string SolutionPrefix { get; set; }
        public static string SolutionPath { get; set; }

        private static string _absolutePath { get => $"{SolutionPath}\\{SolutionPrefix}"; }


        /// <summary>
        /// Generates the project's root folder that contains the solution.
        /// </summary>
        /// <returns></returns>
        public static async Task GenerateRootFolder()
        {
            Console.WriteLine("Checking file path");

            if (!Directory.Exists(_absolutePath))
            {
                Directory.CreateDirectory(_absolutePath);
                Console.WriteLine($"Directory Created: {_absolutePath}");
            }


        }

        public static async Task GenerateSharedProject()
        {
            Console.WriteLine("Generating Shared Project");

            // generated namespace.shared.
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
