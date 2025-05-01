using Bravectl.Service;
namespace BraveCtl
{
    internal class Program
    {
        static async Task Main(string[] arguments)
        {
            ConsoleInputService inputService = new(arguments);
            await inputService.Run();
        }
    }
}
