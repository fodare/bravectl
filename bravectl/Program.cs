using System;
using Bravectl.Service;
using BraveCtl.Model;
using Microsoft.Extensions.Configuration;
namespace BraveCtl
{
    internal class Program
    {
        static void Main(string[] arguments)
        {
            ConsoleInputService inputService = new(arguments);
            inputService.Run();
        }
    }
}
