using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWritingTest
{
    /// <summary>
    /// A class for all of the methods that have to do with input validation.
    /// </summary>
    public static class InputValidationMethods
    {
        /// <summary>
        /// Static string validation method for name fields.
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>The validated input.</returns>
        public static string ValidateName(string? input)
        {
            //Check if name is empty
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("");
            if (input.Equals("b"))
                return input;
            if (input.Equals("r"))
                return input;

            int? maxLength = Config.Instance.Name_MaxLength;
            int? minLength = Config.Instance.Name_MinLength;

            //Check if name contains invalid characters
            if (!input.All(char.IsLetter))
                throw new ArgumentException("Name must only contain letters.");

            //Check if name is too short or too long
            if (input.Length > Config.Instance.Name_MaxLength || input.Length < Config.Instance.Name_MinLength)
                throw new ArgumentException("Name must be at least " +
                    minLength + " characters in length and no more than " +
                    maxLength + " characters in length.");

            return input;
        }

        /// <summary>
        /// Static string validation method for date fields.
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>The validated input.</returns>
        public static string ValidateDate(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("");
            if (input.Equals("b"))
                return input;
            if (input.Equals("r"))
                return input;

            string format = "MM/dd/yyyy";
            DateOnly date;
            if (!DateOnly.TryParseExact(input, format, out date))
                throw new ArgumentException("Invalid date. Check formatting and try again.");

            if (date > DateOnly.FromDateTime(DateTime.Now))
                throw new ArgumentException("Date of birth cannot be in the future.");

            if (ConversionMethods.CalculateAge(date) > Config.Instance.Age_Max)
                throw new ArgumentException("User age cannot be greater than " + Config.Instance.Age_Max);

            return input;
        }

        /// <summary>
        /// Static string validation method for the marital status enum.
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>The validated input.</returns>
        public static string ValidateMaritalStatus(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("");
            if (input.Equals("b"))
                return input;
            if (input.Equals("r"))
                return input;

            if (!int.TryParse(input, out int index))
                throw new ArgumentException("Input must be a number. " +
                    "Consult the list above to determine which number to enter");

            MaritalStatusEnum[] maritalStatusList = Config.Instance.MaritalStatusArray;
            if (index > maritalStatusList.Length || index < 1)
                throw new ArgumentException("Input must correspond with a marital status " +
                    "Consult the list above to determine which number to enter.");

            return input;
        }

        /// <summary>
        /// Static string validation method for booleans.
        /// </summary>
        /// <param name="input">The input to validate.</param>
        /// <returns>The validated input.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static string ValidateBoolean(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException("Please type either y for yes or n for no.");

            if (!input.Equals("Y") && !input.Equals("y") && !input.Equals("N") && !input.Equals("n"))
                throw new ArgumentException("Please type either y for yes or n for no.");

            return input;
        }
    }
}
