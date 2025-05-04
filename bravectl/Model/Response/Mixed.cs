using System;
namespace BraveCtl.Model.Response
{
    public class Mixed
    {
        public string? Type { get; set; }
        public List<ResultReference>? Main { get; set; }
        public List<ResultReference>? Top { get; set; }
        public List<ResultReference>? Side { get; set; }
    }
}