using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GYM_APP.Validations
{
    public class PhoneNumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
                return false;

            var phoneNumber = value.ToString();

            // Basic phone number validation - adjust regex based on your requirements
            var phoneRegex = @"^[\+]?[1-9][\d]{0,15}$";

            return Regex.IsMatch(phoneNumber, phoneRegex);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field is not a valid phone number.";
        }
    }
}

