using System;
namespace BraveCtl.Model.Response
{
    public class Mixed
    {
        public string? Type { get; set; }
        public List<ResultFilter>? Main { get; set; }
        public List<ResultFilter>? Top { get; set; }
        public List<ResultFilter>? Side { get; set; }
    }
}