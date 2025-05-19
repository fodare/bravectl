using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
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
        private readonly string[] knownSubCommands = ["search", "extract"];
        private string[] _cliArguments;
        private ICollection<System.ComponentModel.DataAnnotations.ValidationResult>? _validationResults = null;
        private readonly IBraveAPIService _braveAPIService;
        private readonly IAnsiConsole _console;

        public ConsoleInputService(string[] cliArguments, IBraveAPIService braveAPIService, IAnsiConsole console)
        {
            _cliArguments = cliArguments;
            _braveAPIService = braveAPIService;
            _console = console;
        }

        public async Task Run()
        {
            try
            {
                if (_cliArguments.Length == 0 || _cliArguments.Contains("-h") || _cliArguments.Contains("--help"))
                {
                    await PrintHelp();
                }
                else if (knownSubCommands.Contains(_cliArguments[0]))
                {
                    if (_cliArguments[0] == "search")
                    {
                        _cliArguments = [.. _cliArguments.Where(arguments => arguments != "search")];
                        var parsedInput = await ParseCommandLineArguments();
                        QueryParameters queryParameters = await ConstructQueryParameters(parsedInput);
                        if (IsQueryParameterValid(queryParameters, out _validationResults))
                        {
                            BraveResponse braveResponse = await GetSearchResult(queryParameters);
                            if (braveResponse != null)
                            {
                                await PrintSearchResult(braveResponse, queryParameters.ResultFilter!);
                            }
                        }
                        else
                        {
                            _console.MarkupLine($"[red]{string.Join("\n", _validationResults.Select(result => result.ErrorMessage))}[/]");
                        }
                    }
                    else if (_cliArguments[0] == "extract")
                    {
                        try
                        {
                            string? webPageContent = await StringfyWebPageContent(_cliArguments[1]);
                            if (webPageContent is null || webPageContent == "")
                            {
                                _console.MarkupLine($"[yellow]There are no content to extract from {_cliArguments[1]}.[/]");
                            }
                            else
                            {
                                await PrintPanneledPageContent(ConvertHtmlToText(webPageContent));
                            }
                        }
                        catch (IndexOutOfRangeException)
                        {
                            _console.MarkupLine($"[red]Seems you are missing url to extract from.[/] See --help | -h for more info on usage.");
                        }
                    }
                }
                else
                {
                    _console.MarkupLine("[red]Invalid command / options. Please review option(s) / input(s) provided.[/] See -h | --help for more information on options.");
                }
            }
            catch (ArgumentException)
            {
                _console.MarkupLine($"[red]Invalid command / option(s). Please review option(s) / input(s) provided.[/] See -h | --help for more information on options.");
            }
        }

        public Task PrintHelp()
        {
            _console.MarkupLine("Description: A lightweight C# command-line tool that interacts with the Brave API to facilitate web searches from your CLI.\n");

            _console.MarkupLine("Usage: barvectl [sub-commands] [search-options]\n".EscapeMarkup());
            _console.MarkupLine("  --help, -h          Print help information.\n");

            _console.MarkupLine("  Example: barvectl search --filter web --query \"where is the ISS\" --max 5");
            _console.MarkupLine("  Example: barvectl extract \"https://spotthestation.nasa.gov/\"");

            _console.MarkupLine("\nSub commands:");
            _console.MarkupLine("  search              Search the web for a given search query.");
            _console.MarkupLine("  extract             Convert a given webpage(URL) to text and print to console.");

            _console.MarkupLine("\nSearch options:");
            _console.MarkupLine("  --filter, -f        Result filter. Possible options are videos, web.");
            _console.MarkupLine("  --query, -q         Search query (Max query length 400).");
            _console.MarkupLine("  --country, -c       (Optional: E.g US) Search query country, where the results come from.");
            _console.MarkupLine("  --lang, -l          (Optional: E.g en) Search language preference.");
            _console.MarkupLine("  --interface, -i     (Optional: E.g en-US) User interface language perferred in response.");
            _console.MarkupLine("  --safe, -s          (Optional: E.g off) Filter search results for adult content. Possible options are off, moderate, strict.");
            _console.MarkupLine("  --max, -m           (Optional: E.g 10) Number of returned result. Default count is 5 and max is 20");
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

        public Task<QueryParameters> ConstructQueryParameters(Dictionary<string, string> parsedCmdArguments)
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

        public string? GetParsedArgumentValue(Dictionary<string, string> parsedArguments, string firstKey, string secondKey)
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

        public bool IsQueryParameterValid(QueryParameters queryParameters, out ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults)
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
                _console.MarkupLineInterpolated($"[red]Error parsing brave response.[/] Error message: {exp.Message}");
            }
            catch (HttpRequestException exp)
            {
                _console.MarkupLine($"[red]Barve API error.[/] Error message: {exp.Message}");
            }
            return braveResponse!;
        }

        public Task PrintSearchResult(BraveResponse? braveResponse, string filter = "web")
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
                _console.MarkupLine($"There are no result(s) for the searched query.");
            }
            return Task.CompletedTask;
        }

        public string EscapeHTMLtoString(string htmlString)
        {
            return HttpUtility.HtmlDecode(Markup.Escape(htmlString));
        }

        public async Task<string?> StringfyWebPageContent(string url)
        {
            string webContnet = "";
            try
            {
                using HttpClient httpClient = new();
                webContnet = await httpClient.GetStringAsync(url);
            }
            catch (InvalidOperationException)
            {
                _console.MarkupLine($"[red]Error. Provided url might be invalid, please check and try again.[/]");
            }
            catch (HttpRequestException exception)
            {
                _console.MarkupLine($"[red]Error retriving content from {url}. Error message: {exception.Message}.[/]");
            }
            return webContnet;
        }

        public string ConvertHtmlToText(string htmlContent)
        {
            Regex[] _htmlReplaces = new[] {
                new Regex(@"<script\b[^<]*(?:(?!</script>)<[^<]*)*</script>",
                RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1)),
                new Regex(@"<style\b[^<]*(?:(?!</style>)<[^<]*)*</style>",
                RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1)),
                new Regex(@"<[^>]*>", RegexOptions.Compiled),
                new Regex(@" +", RegexOptions.Compiled)
            };

            foreach (var r in _htmlReplaces)
            {
                htmlContent = r.Replace(htmlContent, " ");
            }
            var lines = htmlContent
                .Split(new[] { '\r', '\n' })
                .Select(_ => WebUtility.HtmlDecode(_.Trim()))
                .Where(_ => _.Length > 0)
                .ToArray();
            return string.Join("\n", lines);
        }

        public Task PrintPanneledPageContent(string text)
        {
            var panel = new Panel(text)
                .Header("Page content")
                .Border(BoxBorder.Rounded)
                .Padding(1, 1, 1, 1)
                .Expand();
            AnsiConsole.Write(panel);
            return Task.CompletedTask;
        }
    }
}
