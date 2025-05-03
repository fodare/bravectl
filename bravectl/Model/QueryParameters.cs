using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Bravectl.Helper;
namespace BraveCtl.Model
{
    public class QueryParameters
    {
        [StringLength(400, MinimumLength = 2, ErrorMessage = "Qurey string can not be longer than {1} / lower than {2} characters.")]
        public string? Q { get; set; }

        public string? Country { get; set; } = "US";

        public string? Search_language { get; set; } = "en";

        [ValidCulture(ErrorMessage = "Provided culture / interface code is not valid. Example expected format de-DE.")]
        public string? UI_Language { get; set; } = "en-US";
        public int Count { get; set; } = 20;

        [ValidateSafeSearch(ErrorMessage = "Provided safe search input is not valid. Possible values are off, moderate, strict.")]
        public string? SafeSearch { get; set; } = "off";
        public string? Freshness { get; set; }
        public bool Spellcheck { get; set; } = true;

        [ValidateResultFilter(ErrorMessage = "Provided filter input is not valid. Possible values are discussions, faq, infobox, news, query, summarizer, videos, web, locations.")]
        public string? ResultFilter { get; set; } = "web";
        public bool Summary { get; set; } = true;
    }
}