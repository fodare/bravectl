using System.ComponentModel.DataAnnotations;
using BraveCtl.Model;
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
        private ICollection<System.ComponentModel.DataAnnotations.ValidationResult>? _validationResults = null;

        public ConsoleInputService(string[] cliArguments)
        {
            _cliArguments = cliArguments;
        }

        public async Task Run()
        {
            try
            {
                if (_cliArguments.Length == 0 || _cliArguments.Contains("-h") || _cliArguments.Contains("--help"))
                {
                    await PrintHelp();
                }
                else if (_cliArguments.Length > 0)
                {
                    var parsedInput = await ParseCommandLineArguments();
                    QueryParameters queryParameters = await ConstructQueryParameters(parsedInput);
                    if (IsQueryParameterValid(queryParameters, out _validationResults))
                    {
                        AnsiConsole.MarkupLine($" Q:{queryParameters.Q}, Country:{queryParameters.Country}, Language:{queryParameters.Search_language}, UI:{queryParameters.UI_Language}, Count:{queryParameters.Count}, Safe:{queryParameters.SafeSearch}, SpellCheck:{queryParameters.Spellcheck}, Filter:{queryParameters.ResultFilter}, Summary:{queryParameters.Summary}");
                    }
                    else
                    {
                        Console.WriteLine(string.Join("\n", _validationResults.Select(result => result.ErrorMessage)));
                    }
                }
            }
            catch (ArgumentException)
            {
                AnsiConsole.MarkupLine($"[red]Invalid option(s). Please review option(s) / input(s) provided.[/] See -h | --help for more information on options.");
            }
        }

        public static Task PrintHelp()
        {
            AnsiConsole.WriteLine("Description: A lightweight C# command-line tool that interacts with the Brave API to facilitate web searches from your CLI.\n");

            AnsiConsole.WriteLine("Usage: barvectl [search-options]\n");

            AnsiConsole.WriteLine("Search options:");
            AnsiConsole.WriteLine("  --help, -h          Print help information.");
            AnsiConsole.WriteLine("  --filter, -f        Result filter. Possible options are discussions, faq, infobox, news, query, summarizer, videos, web, locations.");
            AnsiConsole.WriteLine("  --query, -q         Search query (Max query length 400).");
            AnsiConsole.WriteLine("  --country, -c       (Optional: E.g US) Search query country, where the results come from.");
            AnsiConsole.WriteLine("  --lang, -l          (Optional: E.g en) Search language preference.");
            AnsiConsole.WriteLine("  --interface, -i     (Optional: E.g en-US) User interface language perferred in response.");
            AnsiConsole.WriteLine("  --safe, -s          (Optional: E.g off) Filter search results for adult content. Possible options are off, moderate, strict.");
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

        public static Task<QueryParameters> ConstructQueryParameters(Dictionary<string, string> parsedCmdArguments)
        {
            return Task.Run(() =>
            {
                return new QueryParameters()
                {
                    Q = GetParsedArgumentValue(parsedCmdArguments, "q", "query"),
                    Country = GetParsedArgumentValue(parsedCmdArguments, "c", "country"),
                    Search_language = GetParsedArgumentValue(parsedCmdArguments, "l", "lang"),
                    UI_Language = GetParsedArgumentValue(parsedCmdArguments, "i", "interface"),
                    SafeSearch = GetParsedArgumentValue(parsedCmdArguments, "s", "safe"),
                    ResultFilter = GetParsedArgumentValue(parsedCmdArguments, "f", "filter")!.ToLower()
                };
            });
        }

        public static string? GetParsedArgumentValue(Dictionary<string, string> parsedArguments, string firstKey, string secondKey)
        {
            string value = "";
            if (parsedArguments.ContainsKey(firstKey))
            {
                value = parsedArguments[firstKey];
            }
            else if (parsedArguments.ContainsKey(secondKey))
            {
                value = parsedArguments[secondKey];
            }
            return value;
        }

        public static bool IsQueryParameterValid(QueryParameters queryParameters, out ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults)
        {
            validationResults = [];
            return Validator.TryValidateObject(queryParameters, new ValidationContext(queryParameters), validationResults, true);
        }
    }
}
