using System;

namespace Bravectl.Model.Response
{
    public class Videos
    {
        public string? Type { get; set; }
        public List<VideoResult>? Results { get; set; }
        public bool Mutated_by_goggles { get; set; }
    }
}