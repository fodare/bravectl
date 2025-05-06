using System.ComponentModel.DataAnnotations;

namespace Bravectl.Helper.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateResultFilterAttribute : ValidationAttribute
    {
        private static readonly string[] knownResultFilter = ["videos", "web"];
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