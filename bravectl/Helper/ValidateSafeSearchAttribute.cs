using System.ComponentModel.DataAnnotations;
namespace Bravectl.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidateSafeSearchAttribute : ValidationAttribute
    {
        private static readonly string[] KnownSafeSearch = ["off", "moderate", "strict"];
        public override bool IsValid(object? value)
        {
            if (value is null || string.IsNullOrEmpty((string?)(value)))
            {
                return true;
            }
            else if (value is string && !string.IsNullOrWhiteSpace((string?)value))
            {
                if (KnownSafeSearch.Contains(((string?)value)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}