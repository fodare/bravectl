using System;
using System.Diagnostics.CodeAnalysis;
using Bravectl.Service;
using BraveCtl.Model;
using Moq;
using Spectre.Console;
using System.IO;
using Spectre.Console.Rendering;
using System.Text.RegularExpressions;

namespace bravectlTests
{
    [Collection("Non-Parallel Collection")]
    public class ConsoleInputServiceTests
    {

        [Fact]
        public async Task Run_ShallPrint_HelpMessage_WhenCommandArgument_IsEmpty()
        {
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            Mock<IBraveAPIService> _braveAPiService = new();
            ConsoleInputService _cut = new([], _braveAPiService.Object, console);
            string sampleConsoleMessage = "A lightweight C# command-line tool";

            await _cut.Run();

            var result = output.ToString().Trim();
            string errorDescription = $"Expected '{sampleConsoleMessage}' is present in '{result}.'";
            Assert.True(result.IndexOf(sampleConsoleMessage, StringComparison.OrdinalIgnoreCase) > 0, errorDescription);
        }

        [Theory]
        [InlineData("--filter", "-h")]
        public async Task Run_ShallPrintHelpMessage_WhenUserPassesIn_HelpFlag(params string[] cmdArguments)
        {
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            Mock<IBraveAPIService> _braveAPiService = new();
            ConsoleInputService _cut = new(cmdArguments, _braveAPiService.Object, console);
            string sampleConsoleMessage = "A lightweight C# command-line tool";

            await _cut.Run();

            var result = output.ToString().Trim();
            string errorDescription = $"Expect '{sampleConsoleMessage}' is present in\n\n '{output.ToString()}'";
            Assert.True(result.IndexOf(sampleConsoleMessage, StringComparison.OrdinalIgnoreCase) > 0, errorDescription);
        }

        [Theory]
        [InlineData("--filter", "web", "-q", "What")]
        [InlineData("--filter", "videos", "-q", "What")]
        public async Task ParseCommandLineArguments_ShallReturn_DictionaryOfAll_CmdArguments(params string[] cmdArguments)
        {
            Mock<IBraveAPIService> _braveAPIService = new Mock<IBraveAPIService>();
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            ConsoleInputService _cut = new(cmdArguments, _braveAPIService.Object, console);
            int expectedParsedArgumentDictionaryCount = cmdArguments.Length / 2;

            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            string errorMessage = $"Expected number of parsed command-line args to be {expectedParsedArgumentDictionaryCount} but it was {parsedArguments.Count}";
            Assert.True(expectedParsedArgumentDictionaryCount == parsedArguments.Count, errorMessage);
        }

        [Theory]
        [InlineData("--filter", "web", "-q", "What")]
        public async Task ConstructQueryParameters_ReturnsVlaid_QueryParametersObject(params string[] cmdArguments)
        {
            Mock<IBraveAPIService> _braveAPIService = new Mock<IBraveAPIService>();
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            ConsoleInputService _cut = new(cmdArguments, _braveAPIService.Object, console);

            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            QueryParameters queryParameters = await _cut.ConstructQueryParameters(parsedArguments);

            Assert.Multiple(() =>
            {
                Assert.True(queryParameters.Q != null, $"Q is not expected to be null or empty, but it was '{queryParameters.Q}'.");
                Assert.True(queryParameters.Country != null, $"Country is not expected to be null or empty, bu it was '{queryParameters.Country}'.");
                Assert.True(queryParameters.Search_language != null, $"Search_language is not expected to be null or empty but it was '{queryParameters.Search_language}'.");
                Assert.True(queryParameters.UI_Language != null, $"UI_Language is not expected to be null or empty, but it was '{queryParameters.UI_Language}'.");
                Assert.True(queryParameters.Count < 20, $"Count is expected to be lesser than 20, but it was {queryParameters.Count}.");
                Assert.True(queryParameters.SafeSearch != null, $"SafeSearch is not expected to be null or empty, but it was '{queryParameters.SafeSearch}'.");
                Assert.True(queryParameters.Spellcheck, $"Spellcheck is expected to be true, but it was {queryParameters.Spellcheck}.");
                Assert.True(queryParameters.ResultFilter != null, $"ResultFilter is not expected to be null or empty but it was {queryParameters.ResultFilter}");
                Assert.True(queryParameters.Summary, $"Summary is expected to be true but it was {queryParameters.Summary}.");
            });
        }

        [Theory]
        [InlineData("--filter", "web", "-q", "What")]
        public async Task IsQueryParameterValid_ShallReturnTrue_ForValidQueryParametersObjectAsync(params string[] cmdArguments)
        {
            Mock<IBraveAPIService> _braveAPIService = new Mock<IBraveAPIService>();
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            ConsoleInputService _cut = new(cmdArguments, _braveAPIService.Object, console);

            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            QueryParameters queryParameters = await _cut.ConstructQueryParameters(parsedArguments);

            bool isValid = _cut.IsQueryParameterValid(queryParameters, out ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults);
            Assert.Multiple(() =>
            {
                Assert.True(isValid, $"Given query-parameters validation check is expected to be true but is was {isValid}.");
                Assert.True(validationResults.Count == 0, $"Given query-parameters validation check should return 0 validation result count but it was {validationResults.Count}.");
            });
        }

        [Theory]
        [InlineData("--filter", "web", "-q", "What is your name")]
        [InlineData("--filter", "videos", "-q", "What is your name")]
        public async Task GetSearchResult_ShallReturnValidBraveResponseObjectAsync(params string[] cmdArguments)
        {
            Mock<IBraveAPIService> _braveAPIService = new();
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            ConsoleInputService _cut = new(cmdArguments, _braveAPIService.Object, console);

            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            QueryParameters queryParameters = await _cut.ConstructQueryParameters(parsedArguments);
            _braveAPIService
                .Setup(mock => mock.Search(queryParameters).Result)
                .Returns(new BraveResponse() { Type = $"{cmdArguments[1]}" });

            BraveResponse braveResponse = await _cut.GetSearchResult(queryParameters);

            string expectedBraveResponeType = cmdArguments[1];
            string errorMessage = $"Brave-response type is expected to be {expectedBraveResponeType} but it was {braveResponse.Type}.";
            Assert.True(expectedBraveResponeType == braveResponse.Type, errorMessage);
        }

        [Theory]
        [InlineData("search", "--filter", "web", "-q", "What is your name")]
        [InlineData("search", "--filter", "videos", "-q", "What is your name")]
        public async Task Run_ShallPrint_BraveAPIError_WhenBraveAPIKey_IsMissing(params string[] cmdArguments)
        {
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            IBraveAPIService _braveAPIService = new BraveAPIService();
            ConsoleInputService _cut = new(cmdArguments, _braveAPIService, console);
            string sampleConsoleErrorMessage = "Response status code does not indicate success";

            await _cut.Run();

            var result = output.ToString().Trim();
            string errorDescription = $"Expected ' {sampleConsoleErrorMessage} ' be present in\n\n ' {result} '.";
            Assert.True(result.IndexOf(sampleConsoleErrorMessage, StringComparison.OrdinalIgnoreCase) > 0, errorDescription);
        }

        [Theory]
        [InlineData("extract")]
        public async Task Run_ShallPrint_MissingURLMessage_WhenExtractURL_IsMissing(params string[] cmdArguments)
        {
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            Mock<IBraveAPIService> _braveAPIService = new();
            ConsoleInputService _cut = new(cmdArguments, _braveAPIService.Object, console);
            string sampleConsoleMessage = "See --help | -h for more info on";

            await _cut.Run();

            var result = output.ToString().Trim();
            string errorDescription = $"Expect ' {sampleConsoleMessage} ' be present in\n\n ' {result} '";
            Assert.True(result.IndexOf(sampleConsoleMessage, StringComparison.OrdinalIgnoreCase) > 0, errorDescription);
        }

        [Theory]
        [InlineData("extract", "https://invalidurl.com")]
        public async Task Run_ShallPrint_NoContnetMessage_WhenURL_IsInvalid(params string[] cmdArguments)
        {
            StringWriter output = new();
            var console = AnsiConsole.Create(new AnsiConsoleSettings
            {
                Ansi = AnsiSupport.No,
                Out = new AnsiConsoleOutput(output)
            });
            Mock<IBraveAPIService> _braveAPIService = new();
            ConsoleInputService _cut = new(cmdArguments, _braveAPIService.Object, console);
            string sampleConsoleMessage = "There are no content to extract from";

            await _cut.Run();

            var result = output.ToString().Trim();
            string errorDescription = $"Expect ' {sampleConsoleMessage} ' be present in\n\n ' {result} '";
            Assert.True(result.IndexOf(sampleConsoleMessage, StringComparison.OrdinalIgnoreCase) > 0, errorDescription);
        }
    }
}