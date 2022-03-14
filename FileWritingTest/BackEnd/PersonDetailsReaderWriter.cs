using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace FileWritingTest
{
    /// <summary>
    /// Static class for writing the contents of a PersonDetails object to a file or reading from that file.
    /// </summary>
    public static class PersonDetailsReaderWriter
    {
        /// <summary>
        /// Retrieves one PersonDetail from the file at fileName
        /// </summary>
        /// <param name="fileName">File path</param>
        /// <returns>The PersonDetails List as read from the CSV file</returns>
        public static List<PersonDetails> GetPeopleDetails()
        {
            IFileReaderWriter? fileParser = DetermineFileParser();
            List<PersonDetails> parsedPeopleDetails;
            if (fileParser != null)
            {
                parsedPeopleDetails = fileParser.GetPeopleDetails();
            }
            else
                throw new InvalidOperationException("File type unsupported");
            return parsedPeopleDetails;
        }

        /// <summary>
        /// Method for writing PersonDetails entries into a file.
        /// </summary>
        /// <param name="person">The PersonDetails object to be written.</param>
        public static void WritePersonDetails(PersonDetails person)
        {
            IFileReaderWriter? fileParser = DetermineFileParser();
            if (fileParser != null)
            {
                fileParser.WritePersonDetails(person);
            }
            else
                throw new InvalidOperationException("File type unsupported");
        }

        /// <summary>
        /// Determines the filetype of the file at fileName and returns
        /// a valid FileParser for that filetype
        /// </summary>
        /// <returns>The FileParser for that file path</returns>
        private static IFileReaderWriter? DetermineFileParser()
        {
            string fileExtension = Strings.PeopleFileName.Split(".")[1];
            IFileReaderWriter? fileParser = null;

            if (fileExtension == "txt")
                fileParser = new CsvReaderWriter();
            else if (fileExtension == "xml")
                fileParser = new XmlReaderWriter();
            else if (fileExtension == "json")
                fileParser = new JsonFileParser();

            return fileParser;
        }
    }
}
