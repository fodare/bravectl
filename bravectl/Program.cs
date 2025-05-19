using Bravectl.Service;
using Spectre.Console;
namespace BraveCtl
{
    internal class Program
    {
        static async Task Main(string[] arguments)
        {
            IBraveAPIService braveAPIService = new BraveAPIService();
            ConsoleInputService inputService = new(arguments, braveAPIService, AnsiConsole.Console);
            await inputService.Run();
        }
    }
}
