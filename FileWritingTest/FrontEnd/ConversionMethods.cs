using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWritingTest
{
    /// <summary>
    /// Helpful methods for converting inputed strings of inputed values.
    /// </summary>
    public static class ConversionMethods
    {
        /// <summary>
        /// Method for converting a string in mm/dd/yyyy format to a DateOnly object
        /// </summary>
        /// <param name="date">The string to be converted. Must be in mm/dd/yyyy format.</param>
        /// <returns>The date string as a DateOnly object</returns>
        /// <exception cref="ArgumentException">If the input string format is incorrect</exception>
        public static DateOnly ConvertDate(string input)
        {
            string format = "MM/dd/yyyy";
            if (!DateOnly.TryParseExact(input, format, out DateOnly date))
            {
                throw new ArgumentException("ConvertDate: Invalid date format. Must be in MM/dd/yyyy format.");
            }
            return date;
        }

        /// <summary>
        /// Method for converting a string that contains a number representing an index in the
        /// MaritalStatusArray read from the config file. Returns the MaritalStatusEnum at that
        /// index.
        /// </summary>
        /// <param name="index">String containing an index in the MaritalStatusArray.</param>
        /// <returns>The MaritalStatusEnum associated with the passed index.</returns>
        public static MaritalStatusEnum ConvertMaritalStatus(string index)
        {
            MaritalStatusEnum maritalStatus;
            try
            {
                int maritalStatusIndex = int.Parse(index);
                maritalStatus = Config.Instance.MaritalStatusArray[maritalStatusIndex - 1];
            }
            catch (IndexOutOfRangeException)
            {
                throw new ArgumentException("ConvertMaritalStatus: Passed index " + index + " is not in the MaritalStatusArray.");
            }
            catch (FormatException)
            {
                throw new ArgumentException("ConvertMaritalStatus: Passed string " + index + " cannot be parsed to int");
            }
            return maritalStatus;
        }

        /// <summary>
        /// Method for converting y/n or Y/N answers to boolean.
        /// Only y or Y will return true.
        /// </summary>
        /// <param name="input">String to convert to boolean.</param>
        /// <returns>True if input string is "y" or "Y". False Otherwise.</returns>
        public static bool ConvertYesNo(string input)
        {
            if (input.Equals("y") || input.Equals("Y"))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculates the age of a user given a date of birth in DateOnly format.
        /// </summary>
        /// <param name="birthDay">The date of birth of the user.</param>
        /// <returns>The age of a user in years</returns>
        public static int CalculateAge(DateOnly birthDay)
        {
            DateTime today = DateTime.Today;
            DateTime birthDateTime = birthDay.ToDateTime(new TimeOnly(0, 0));
            int age = today.Year - birthDateTime.Year;
            if (birthDateTime.Date > today.AddYears(-age))
                age--;
            return age;
        }
    }
}
