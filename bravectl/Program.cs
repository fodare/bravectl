using Bravectl.Service;
namespace BraveCtl
{
    internal class Program
    {
        static async Task Main(string[] arguments)
        {
            IBraveAPIService braveAPIService = new BraveAPIService();
            ConsoleInputService inputService = new(arguments, braveAPIService);
            await inputService.Run();
        }
    }
}
