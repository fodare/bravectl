namespace BraveCtl.Model.Response
{
    public class Query
    {
        public string? Original { get; set; }
        public bool Show_Strict_Warning { get; set; }
        public bool Is_navigational { get; set; }
        public bool Is_news_breaking { get; set; }
        public bool Spellcheck_off { get; set; }
        public string? Country { get; set; }
        public bool Bad_results { get; set; }
        public bool Should_fallback { get; set; }
        public string? Postal_code { get; set; }
        public string? City { get; set; }
        public string? Header_country { get; set; }
        public bool More_results_available { get; set; }
        public string? State { get; set; }
    }
}