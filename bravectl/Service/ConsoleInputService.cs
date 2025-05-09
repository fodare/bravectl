using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Web;
using Bravectl.Model.Response;
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
            { "m", "max"},
            { "help", "h" },
            { "filter", "f" },
            { "query", "q" },
            { "country", "c" },
            { "lang", "l" },
            { "interface", "i" },
            { "safe", "s" },
            { "max", "m"}
        };
        private readonly string[] _cliArguments;
        private ICollection<System.ComponentModel.DataAnnotations.ValidationResult>? _validationResults = null;
        private readonly IBraveAPIService _braveAPIService;

        public ConsoleInputService(string[] cliArguments, IBraveAPIService braveAPIService)
        {
            _cliArguments = cliArguments;
            _braveAPIService = braveAPIService;
        }

        public async Task Run()
        {
            try
            {
                if (_cliArguments.Length == 0 || _cliArguments.Contains("-h") || _cliArguments.Contains("--help"))
                {
                    await PrintHelp();
                }
                else
                {
                    var parsedInput = await ParseCommandLineArguments();
                    QueryParameters queryParameters = await ConstructQueryParameters(parsedInput);
                    if (IsQueryParameterValid(queryParameters, out _validationResults))
                    {
                        BraveResponse braveResponse = await GetSearchResult(queryParameters);
                        await PrintSearchResult(braveResponse, queryParameters.ResultFilter!);
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[red]{string.Join("\n", _validationResults.Select(result => result.ErrorMessage))}[/]");
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

            AnsiConsole.WriteLine("  --help, -h          Print help information.\n");

            AnsiConsole.WriteLine("Search options:");
            AnsiConsole.WriteLine("  --filter, -f        Result filter. Possible options are videos, web.");
            AnsiConsole.WriteLine("  --query, -q         Search query (Max query length 400).");
            AnsiConsole.WriteLine("  --country, -c       (Optional: E.g US) Search query country, where the results come from.");
            AnsiConsole.WriteLine("  --lang, -l          (Optional: E.g en) Search language preference.");
            AnsiConsole.WriteLine("  --interface, -i     (Optional: E.g en-US) User interface language perferred in response.");
            AnsiConsole.WriteLine("  --safe, -s          (Optional: E.g off) Filter search results for adult content. Possible options are off, moderate, strict.");
            AnsiConsole.WriteLine("  --max, -m           (Optional: E.g 10) Number of returned result. Default count is 5 and max is 20");
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

                    Country = string.IsNullOrWhiteSpace(GetParsedArgumentValue(parsedCmdArguments, "c", "country")) ? "US" : GetParsedArgumentValue(parsedCmdArguments, "c", "country")!.ToUpper(),

                    Search_language = string.IsNullOrWhiteSpace(GetParsedArgumentValue(parsedCmdArguments, "l", "lang")) ? "en" : GetParsedArgumentValue(parsedCmdArguments, "l", "lang")!.ToLower(),

                    UI_Language = string.IsNullOrWhiteSpace(GetParsedArgumentValue(parsedCmdArguments, "i", "interface")) ? "en-US" : GetParsedArgumentValue(parsedCmdArguments, "i", "interface"),

                    SafeSearch = string.IsNullOrWhiteSpace(GetParsedArgumentValue(parsedCmdArguments, "s", "safe")) ? "off" : GetParsedArgumentValue(parsedCmdArguments, "s", "safe"),

                    ResultFilter = GetParsedArgumentValue(parsedCmdArguments, "f", "filter")!.ToLower(),

                    Count = int.TryParse(GetParsedArgumentValue(parsedCmdArguments, "m", "max")!, out int max) ? max : 5
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

        public async Task<BraveResponse> GetSearchResult(QueryParameters queryParameters)
        {
            BraveResponse? braveResponse = new();
            try
            {
                braveResponse = await _braveAPIService.Search(queryParameters);
            }
            catch (JsonException exp)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Error parsing brave response.[/] Error message: {exp.Message}");
            }
            catch (HttpRequestException exp)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]Barve API error.[/] Error message: {exp.Message}");
            }
            return braveResponse!;
        }

        public static Task PrintSearchResult(BraveResponse? braveResponse, string filter = "web")
        {
            if (filter == "web" && braveResponse!.Web != null)
            {
                var webTable = new Table()
                {
                    Title = new TableTitle("Search result"),
                    ShowRowSeparators = true
                };
                webTable.AddColumns("[bold]Domain[/]", "[bold]Title[/]", "[bold]Short summary[/]", "[bold]URL[/]");
                foreach (SearchResult searchResult in braveResponse.Web.Results!)
                {
                    webTable.AddRow(
                        searchResult!.Profile!.Long_name!,
                        EscapeHTMLtoString(searchResult!.Title!),
                        EscapeHTMLtoString(searchResult!.Description!),
                        searchResult!.Url!);
                }
                AnsiConsole.Write(webTable);
            }
            else if (filter == "videos" && braveResponse!.Videos != null)
            {
                var videosTable = new Table()
                {
                    Title = new TableTitle("Search result"),
                    ShowRowSeparators = true
                };
                videosTable.AddColumns("[bold]Domain[/]", "[bold]Title[/]", "[bold]Short summary[/]", "[bold]URL[/]");
                foreach (VideoResult videoResult in braveResponse.Videos.Results!)
                {
                    videosTable.AddRow(
                        videoResult!.Meta_Url!.Hostname!,
                        EscapeHTMLtoString(videoResult!.Title!),
                        EscapeHTMLtoString(videoResult!.Description!),
                        videoResult!.Url!);
                }
                AnsiConsole.Write(videosTable);
            }
            else
            {
                AnsiConsole.MarkupLine($"There are no result(s) for {braveResponse!.Query!.Original}.");
            }
            return Task.CompletedTask;
        }

        public static string EscapeHTMLtoString(string htmlString)
        {
            return HttpUtility.HtmlDecode(Markup.Escape(htmlString));
        }
    }
}
