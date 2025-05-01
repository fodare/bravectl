using System;

namespace Bravectl.Model.Response
{
    public class SearchResult
    {
        public string? Title { get; set; }
        public string? Url { get; set; }
        public bool Is_source_local { get; set; }
        public bool Is_source_both { get; set; }
        public string? Description { get; set; }

        public string? Page_age { get; set; }
        public SearchResultProfile? Profile { get; set; }
        public string? Language { get; set; }
        public bool Family_friendly { get; set; }
        public string? Type { get; set; }
        public string? Subtype { get; set; }
        public bool Is_live { get; set; }
        public DeepResults? DeepResults { get; set; }
        public MetaUrl? MetaUrl { get; set; }
        public string? Age { get; set; }
    }
}