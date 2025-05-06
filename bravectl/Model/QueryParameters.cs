using System.ComponentModel.DataAnnotations;
using Bravectl.Helper.CustomAttributes;
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

        [Range(1, 20, ErrorMessage = "Serch result count ranges between {1} - {2}.")]
        public int Count { get; set; } = 5;

        [ValidateSafeSearch(ErrorMessage = "Provided safe search input is not valid. Possible values are off, moderate, strict.")]
        public string? SafeSearch { get; set; } = "off";
        public string? Freshness { get; set; }
        public bool Spellcheck { get; set; } = true;

        [ValidateResultFilter(ErrorMessage = "Provided filter input is not valid. Possible values are  videos, web.")]
        public string? ResultFilter { get; set; } = "web";
        public bool Summary { get; set; } = true;
    }
}