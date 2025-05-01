using System;
using System.Reflection.Metadata;
namespace Bravectl.Model.Response
{
    public class WebSearch
    {
        public string? Type { get; set; }
        public List<SearchResult>? Results { get; set; }
        public bool Family_fiendly { get; set; }
    }
}