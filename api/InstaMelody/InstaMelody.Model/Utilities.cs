using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace InstaMelody.Model
{
    public static class Utilities
    {
        /// <summary>
        /// Removes the special characters from a string.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <returns></returns>
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '.' || c == '_')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Determines whether a string is a valid email address format.
        /// </summary>
        /// <param name="emailString">The email string.</param>
        /// <returns></returns>
        public static bool IsValidEmail(string emailString)
        {
            bool isEmail = Regex.IsMatch(emailString,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);

            return isEmail;
        }

        /// <summary>
        /// Validates the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        public static IEnumerable<string> Validate(object o)
        {
            var properties = o.GetType().GetProperties();

            foreach (var propertyInfo in properties)
            {
                var customAttributes = propertyInfo.GetCustomAttributes(typeof(ValidationAttribute), true);
                foreach (var customAttribute in customAttributes)
                {
                    var validationAttribute = (ValidationAttribute)customAttribute;
                    var isValid = validationAttribute.IsValid(propertyInfo.GetValue(o, BindingFlags.GetProperty, null, null, null));

                    if (!isValid)
                    {
                        yield return validationAttribute.ErrorMessage;
                    }
                }
            }
        }
    }
}
