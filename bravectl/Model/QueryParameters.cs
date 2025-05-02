using System.ComponentModel.DataAnnotations;
namespace BraveCtl.Model
{
    public class QueryParameters
    {
        [StringLength(400, MinimumLength = 1, ErrorMessage = "Qurey string can not be longer than {1}.")]
        public string? Q { get; set; }
        public string? Country { get; set; }
        public string? Search_language { get; set; }
        public string? UI_Language { get; set; }
        public int Count { get; set; } = 20;
        public string? SafeSearch { get; set; }
        public string? Freshness { get; set; }
        public bool Spellcheck { get; set; } = true;
        public string? ResultFilter { get; set; }
        public bool Summary { get; set; } = true;
    }
}