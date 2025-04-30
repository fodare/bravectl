using Bravectl.Model.Response;
using BraveCtl.Model.Response;

namespace BraveCtl.Model
{
    public class BraveResponse
    {
        public Query? Query { get; set; }
        public Mixed? Mixed { get; set; }
        public string? Type { get; set; }
        public Videos? Videos { get; set; }
        public WebSearch? Web { get; set; }
    }
}