using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileWritingTest
{
    /// <summary>
    /// Enum representing all possible marital status values
    /// </summary>
    public enum MaritalStatusEnum
    {
        Single,
        Married,
        Separated,
        Divorced,
        Widowed,
        Partnered
    }

    /// <summary>
    /// Singleton object for delivering configuration information across the application
    /// </summary>
    public sealed class Config
    {
        #region Fields
        //Singleton instance
        private static readonly Config instance = new();

        public static Config Instance
        {
            get { return instance; }
        }

        //Configuration values
        //Readonly to prevent changing elsewhere in the project
        //Nullable to differentiate default values from explicit declarations
        public int EmployeePin { get; }
        public int Name_MinLength { get; }
        public int Name_MaxLength { get; }
        public int Age_Max { get; }
        public int Age_DenyThreshold { get; }
        public int Age_AuthThreshold { get; }
        public MaritalStatusEnum[] MaritalStatusArray { get; }
        #endregion

        #region Constructor
        //Private constructor
        private Config()
        {
            //Read all lines from configuration file
            string[] raw = System.IO.File.ReadAllLines("Data/config.txt");
            MaritalStatusArray = Array.Empty<MaritalStatusEnum>();

            //Search for keywords and their values
            foreach (var line in raw)
            {
                if (line.Contains(nameof(Name_MinLength)))                   //check for field
                    Name_MinLength = int.Parse(line.Split('=')[1].Trim());  //isolate, trim, and parse value
                if (line.Contains(nameof(Name_MaxLength)))
                    Name_MaxLength = int.Parse(line.Split('=')[1].Trim());
                if (line.Contains(nameof(Age_Max)))
                    Age_Max = int.Parse(line.Split('=')[1].Trim());
                if (line.Contains(nameof(Age_DenyThreshold)))
                    Age_DenyThreshold = int.Parse(line.Split('=')[1].Trim());
                if (line.Contains(nameof(Age_AuthThreshold)))
                    Age_AuthThreshold = int.Parse(line.Split('=')[1].Trim());
                if (line.Contains(nameof(MaritalStatusArray)))
                {
                    string[] maritalStatusStrings = line.Split('=')[1].Trim().Split(','); //parse statuses
                    MaritalStatusArray = maritalStatusStrings.Select(                     //convert
                        a => Enum.Parse<MaritalStatusEnum>(a.Trim())
                        ).ToArray();
                }
            }

            //Assign all missing values to defaults
            if (Name_MinLength == 0)
                Name_MinLength = DefaultConfig.Name_MinLength;
            if (Name_MaxLength == 0)
                Name_MaxLength = DefaultConfig.Name_MaxLength;
            if (Age_Max == 0)
                Age_Max = DefaultConfig.Age_Max;
            if (Age_DenyThreshold == 0)
                Age_DenyThreshold = DefaultConfig.Age_DenyThreshold;
            if (Age_AuthThreshold == 0)
                Age_AuthThreshold = DefaultConfig.Age_AuthThreshold;
            if (MaritalStatusArray.Length == 0)
                MaritalStatusArray = Enum.GetValues<MaritalStatusEnum>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Prints all Marital Status values to a string, each value labeled with its index.
        /// </summary>
        /// <returns>The string containing all Marital Status values</returns>
        public string PrintMaritalStatuses()
        {
            StringBuilder sb = new();
            int index = 1;
            foreach (var maritalStatus in MaritalStatusArray)
            {
                sb.AppendLine(index + ". " + maritalStatus);
                index++;
            }
            return sb.ToString();
        }

        /// <summary>
        /// Prints the contents of the configuration file for diagnostic purposes.
        /// </summary>
        /// <returns>The contents of the configuration file as a string.</returns>
        public string PrintConfig()
        {
            StringBuilder sb = new();
            sb.AppendLine("Current configuration:");
            sb.AppendLine("");
            sb.AppendLine("Name_MinLength = " + Name_MinLength);
            sb.AppendLine("Name_MaxLength = " + Name_MaxLength);
            sb.AppendLine("Age_Max = " + Age_Max);
            sb.AppendLine("Age_DenyThreshold = " + Age_DenyThreshold);
            sb.AppendLine("Age_AuthThreshold = " + Age_AuthThreshold);
            sb.AppendLine("MaritalStatuses = " + string.Join(",", MaritalStatusArray));
            sb.AppendLine("");
            return sb.ToString();
        }
        #endregion

        #region Classes
        //Default values for Config
        private class DefaultConfig
        {
            public const int Name_MinLength = 2;
            public const int Name_MaxLength = 25;
            public const int Age_Max = 150;
            public const int Age_DenyThreshold = 16;
            public const int Age_AuthThreshold = 18;
        }
        #endregion
    }
}
