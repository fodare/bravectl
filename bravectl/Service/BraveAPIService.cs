using System.Net.Http.Json;
using BraveCtl.Model;

namespace Bravectl.Service
{
    public class BraveAPIService : IBraveAPIService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _braveAPIKey;

        public BraveAPIService()
        {
            _httpClient = new HttpClient();
            _braveAPIKey = Environment.GetEnvironmentVariable("braveAPIKey");
        }

        public async Task<BraveResponse?> Search(QueryParameters queryParameters)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.search.brave.com/res/v1/web/search?q={queryParameters.Q}&country={queryParameters.Country}&search_lang={queryParameters.Search_language}&ui_lang={queryParameters.UI_Language}&safesearch={queryParameters.SafeSearch}&result_filter={queryParameters.ResultFilter}&count={queryParameters.Count}");
            request.Headers.Add("X-Subscription-Token", $"{_braveAPIKey}");
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<BraveResponse>();
        }
    }
}