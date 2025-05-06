using System;
namespace Bravectl.Model.Response
{
    public class VideoResult
    {
        public string? Type { get; set; }
        public string? Url { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Age { get; set; }
        public string? Page_age { get; set; }
        public Video? Video { get; set; }
        public MetaUrl? Meta_Url { get; set; }
        public Thumbnail? Thumbnail { get; set; }
    }
}