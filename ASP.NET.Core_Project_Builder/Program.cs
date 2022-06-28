﻿using ASP.NET.Core_Project_Builder;

var inputs = false;

while (!inputs)
{
    CodeGenerator.SolutionPrefix = null;
    CodeGenerator.SolutionPath = null;

    while (string.IsNullOrWhiteSpace(CodeGenerator.SolutionPrefix))
    {
        Console.Clear();

        Console.Write("Solution Name: ");

        CodeGenerator.SolutionPrefix = Console.ReadLine();

    }

    while (string.IsNullOrWhiteSpace(CodeGenerator.SolutionPath))
    {
        Console.Clear();

        Console.Write("Save location (this will generate the folder for you): ");

        CodeGenerator.SolutionPath = Console.ReadLine();
    }

    Console.WriteLine($"Your solution prefix will be {CodeGenerator.SolutionPrefix}");
    Console.WriteLine($"and will be saved to {CodeGenerator.SolutionPath}\\{CodeGenerator.SolutionPrefix}");

    Console.Write("Would you like to proceed? (Y/N): ");
    var key = Console.ReadKey();

    if (key.Key == ConsoleKey.Y)
    {
        inputs = true;
    }

    
}

await CodeGenerator.GenerateRootFolder();