using Spectre.Console;
namespace Bravectl.Service
{
    public class ConsoleInputService
    {
        private readonly Dictionary<string, string> _aliases = new()
        {
            { "h", "help" },
            { "f", "filter" },
            { "q", "query" },
            { "c", "country" },
            { "l", "lang" },
            { "i", "interface" },
            { "s", "safe" },
            { "t", "truncate"},
            { "help", "h" },
            { "filter", "f" },
            { "query", "q" },
            { "country", "c" },
            { "lang", "l" },
            { "interface", "i" },
            { "safe", "s" },
            { "truncate","t"}
        };
        private readonly string[] _cliArguments;

        public ConsoleInputService(string[] cliArguments)
        {
            _cliArguments = cliArguments;
        }

        public async Task Run()
        {
            try
            {
                if (_cliArguments.Length == 0)
                {
                    await PrintHelp();
                }
                else if (_cliArguments.Length > 0)
                {
                    var parsedInput = await ParseCommandLineArguments();
                    foreach (var input in parsedInput)
                    {
                        Console.WriteLine(input.ToString());
                    }
                }
            }
            catch (ArgumentException)
            {
                AnsiConsole.MarkupLine($"[red]Invalid option(s). Please review option(s) / input(s) provided.[/] See -h | --help for more information on options.");
            }
        }

        public Task PrintHelp()
        {
            AnsiConsole.WriteLine("Description: A lightweight C# command-line tool that interacts with the Brave API to facilitate web searches from your CLI.\n");

            AnsiConsole.WriteLine("Usage: barvectl [search-options]\n");

            AnsiConsole.WriteLine("Search options:");
            AnsiConsole.WriteLine("  --help, -h          Print help information.");
            AnsiConsole.WriteLine("  --filter, -f        Result filter. Possible options are discussions, videos, locations, faq, new,summarizer, web.");
            AnsiConsole.WriteLine("  --query, -q         Search query (Max query length 400).");
            AnsiConsole.WriteLine("  --country, -c       (Optional: E.g US) Search query country, where the results come from.");
            AnsiConsole.WriteLine("  --lang, -l          (Optional: E.g en) Search language preference.");
            AnsiConsole.WriteLine("  --interface, -i     (Optional: E.g en-US) User interface language perferred in response.");
            AnsiConsole.WriteLine("  --safe, -s          (Optional: E.g off) Filter search results for adult content. Possible options are off, moderate, strict.");
            AnsiConsole.WriteLine("  --truncate,-t       (Optional: E.g true) Enables suumery generation for web search. ");
            return Task.CompletedTask;
        }

        public Task<Dictionary<string, string>> ParseCommandLineArguments()
        {
            return Task.Run(() =>
            {
                var parsedResult = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (int i = 0; i < _cliArguments.Length; i++)
                {
                    var argument = _cliArguments[i];
                    if (argument.StartsWith("--") || argument.StartsWith("-"))
                    {
                        var trimmed = argument.TrimStart('-');
                        // Apply alias mapping if available
                        if (_aliases != null && _aliases.ContainsKey(trimmed))
                        {
                            trimmed = _aliases[trimmed];
                        }
                        else
                        {
                            throw new ArgumentException();
                        }
                        // Check if next item is a value (and not another flag)
                        if (i + 1 < _cliArguments.Length && !_cliArguments[i + 1].StartsWith("-"))
                        {
                            parsedResult[trimmed] = _cliArguments[i + 1];
                            i++; // Skip next since it's a value
                        }
                        else
                        {
                            // It's a switch/flag without a value (treated as true)
                            parsedResult[trimmed] = "true";
                        }
                    }
                    else
                    {
                        throw new ArgumentException();
                    }
                }
                return parsedResult;
            });
        }
    }
}
