using Microsoft.SqlServer.Server;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace GreenGuard.Helpers
{
    public class DateValidation
    {
        private static readonly string[] _formats = { "MM-dd-yyyy", "MM/dd/yyyy", "dd.MM.yyyy", "dd.MM.yyyy", "dd/MM/yyyy" };

        public class PastDateAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var date = (DateTime)value;
                if (date.Date >= DateTime.Now.Date)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult(ErrorMessage);
                }
            }
        }

        public class ValidDateFormatAttribute : ValidationAttribute
        {
            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var dateValue = value.ToString();

                if (dateValue == null)
                {
                    return new ValidationResult("Значення не є рядком");
                }

                DateTime parsedDate;
                if (DateTime.TryParseExact(dateValue, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult(ErrorMessage);
            }
        }

        public static bool TryParseDate(string dateString, out DateTime date)
        {
            return DateTime.TryParseExact(dateString, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
        }
    }
}
