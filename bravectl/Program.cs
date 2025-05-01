using Bravectl.Service;
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
