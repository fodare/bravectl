using System.ComponentModel.DataAnnotations;
using Bravectl.Helper;
namespace BraveCtl.Model
{
    public class QueryParameters
    {
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Qurey string can not be longer than {1}.")]
        public string? Q { get; set; }

        [StringLength(2, MinimumLength = 2, ErrorMessage = "Country code can not be lower / longer than {1} charcters.")]
        public string? Country { get; set; }

        [StringLength(2, MinimumLength = 2, ErrorMessage = "Search language can not be lower /  longer the {1} charcters.")]
        public string? Search_language { get; set; }

        [ValidCulture(ErrorMessage = "Provided culture / interface code is not valid. Example format de-DE.")]
        public string? UI_Language { get; set; }
        public int Count { get; set; } = 20;

        [ValidateSafeSearch(ErrorMessage = "Provided safe search input is not valid. Possible values are off, moderate, strict.")]
        public string? SafeSearch { get; set; }
        public string? Freshness { get; set; }
        public bool Spellcheck { get; set; } = true;

        [ValidateResultFilter(ErrorMessage = "Provided filter input is not valid. Possible values are discussions, faq, infobox, news, query, summarizer, videos, web, locations.")]
        public string? ResultFilter { get; set; }
        public bool Summary { get; set; } = true;
    }
}