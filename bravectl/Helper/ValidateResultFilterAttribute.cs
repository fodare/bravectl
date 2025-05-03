using System.ComponentModel.DataAnnotations;

namespace Bravectl.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateResultFilterAttribute : ValidationAttribute
    {
        private static readonly string[] knownResultFilter = ["discussions", "faq", "infobox", "news", "query", "summarizer", "videos", "web", "locations"];
        public override bool IsValid(object? value)
        {
            if (value is string && !string.IsNullOrWhiteSpace((string?)value))
            {
                if (knownResultFilter.Contains(((string?)value)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}