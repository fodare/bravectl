using System.ComponentModel.DataAnnotations;
using System.Globalization;
namespace Bravectl.Helper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ValidCultureAttribute : ValidationAttribute
    {
        private static readonly string[] KnownCultureNames = [.. CultureInfo
        .GetCultures(CultureTypes.SpecificCultures | CultureTypes.NeutralCultures)
        .Select(c => c.Name)];

        public override bool IsValid(object? value)
        {
            if (value is null || string.IsNullOrEmpty((string?)(value)))
            {
                return true;
            }
            else if (value is string cultureCode && !string.IsNullOrWhiteSpace(cultureCode))
            {
                return KnownCultureNames.Contains(cultureCode, StringComparer.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}