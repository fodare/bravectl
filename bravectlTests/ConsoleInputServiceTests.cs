using Bravectl.Service;
using BraveCtl.Model;
using Moq;

namespace bravectlTests
{
    public class ConsoleInputServiceTests
    {
        private Mock<IBraveAPIService> _braveAPIService;
        private ConsoleInputService _cut;
        public ConsoleInputServiceTests()
        {
            _braveAPIService = new Mock<IBraveAPIService>();
            _cut = new([], _braveAPIService.Object);
        }

        [Theory]
        [InlineData("--filter", "-h")]
        public async Task RunShallPrintHelpMessage_UserPassesIn_HelpFlag(params string[] cmdArguments)
        {
            _cut = new ConsoleInputService(cmdArguments, _braveAPIService.Object);
            StringWriter stringWriter = new();
            Console.SetOut(stringWriter);
            string sampleHelpMessageContent = "Print help information";

            await _cut.Run();

            var output = stringWriter.ToString();
            string errorDescription = $"Expect '{sampleHelpMessageContent}' is present in\n\n '{output.ToString()}'";
            Assert.True(output.IndexOf(sampleHelpMessageContent, StringComparison.OrdinalIgnoreCase) > 0, errorDescription);
        }

        [Theory]
        [InlineData("--filter", "web", "-q", "What")]
        public async Task ParseCommandLineArguments_ShallReturn_DictionaryOfAll_CmdArguments(params string[] cmdArguments)
        {
            _cut = new ConsoleInputService(cmdArguments, _braveAPIService.Object);
            int expectedParsedArgumentDictionaryCount = cmdArguments.Length / 2;

            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            string errorMessage = $"Expected number of parsed command-line args to be {expectedParsedArgumentDictionaryCount} but it was {parsedArguments.Count}";
            Assert.True(expectedParsedArgumentDictionaryCount == parsedArguments.Count, errorMessage);
        }

        [Theory]
        [InlineData("--filter", "web", "-q", "What")]
        public async Task ConstructQueryParameters_ReturnsVlaid_QueryParametersObject(params string[] cmdArguments)
        {
            _cut = new ConsoleInputService(cmdArguments, _braveAPIService.Object);

            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            QueryParameters queryParameters = await ConsoleInputService.ConstructQueryParameters(parsedArguments);

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
            _cut = new ConsoleInputService(cmdArguments, _braveAPIService.Object);
            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            QueryParameters queryParameters = await ConsoleInputService.ConstructQueryParameters(parsedArguments);

            bool isValid = ConsoleInputService.IsQueryParameterValid(queryParameters, out ICollection<System.ComponentModel.DataAnnotations.ValidationResult> validationResults);
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
            Dictionary<string, string> parsedArguments = await _cut.ParseCommandLineArguments();
            QueryParameters queryParameters = await ConsoleInputService.ConstructQueryParameters(parsedArguments);
            _braveAPIService
                .Setup(mock => mock.Search(queryParameters).Result)
                .Returns(new BraveResponse() { Type = $"{cmdArguments[1]}" });
            _cut = new ConsoleInputService(cmdArguments, _braveAPIService.Object);


            BraveResponse braveResponse = await _cut.GetSearchResult(queryParameters);

            string expectedBraveResponeType = cmdArguments[1];
            string errorMessage = $"Brave-response type is expected to be {expectedBraveResponeType} but it was {braveResponse.Type}.";
            Assert.True(expectedBraveResponeType == braveResponse.Type, errorMessage);
        }
    }
}