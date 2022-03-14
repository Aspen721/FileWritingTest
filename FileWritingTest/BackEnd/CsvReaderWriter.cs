using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileWritingTest
{
    public class CsvReaderWriter : IFileReaderWriter
    {
        private readonly CsvConfiguration config = new(CultureInfo.InvariantCulture)
        {
            Delimiter = "|",
            HasHeaderRecord = false,
            HeaderValidated = null,
            MissingFieldFound = null,
        };

        public CsvReaderWriter()
        {
            if (!Directory.Exists(Strings.DataDirectory))
                Directory.CreateDirectory(Strings.DataDirectory);
            if (!File.Exists(Strings.PeopleFileName))
            {
                StreamWriter writer = File.CreateText(Strings.PeopleFileName);
                writer.Close();
            }
            if (!File.Exists(Strings.SpousesFileName))
            {
                StreamWriter writer = File.CreateText(Strings.SpousesFileName);
                writer.Close();
            }
        }

        /// <summary>
        /// Reads the people.txt CSV file at filePath into an PersonDetails object
        /// </summary>
        /// <returns> The generated PersonDetails object </returns>
        public List<PersonDetails> GetPeopleDetails()
        {
            if(new FileInfo(Strings.PeopleFileName).Length == 0)
                return new List<PersonDetails>();

            List<PersonDetails> result = new();
            using (var streamReader = new StreamReader(Strings.PeopleFileName))
            using (var csvReader = new CsvReader(streamReader, config))
            {
                csvReader.Context.RegisterClassMap<PeopleDetailsMap>();
                var record = csvReader.GetRecords<PersonDetails>();
                result = record.ToList();
            }

            return result;
        }

        /// <summary>
        /// Writes a PersonDetails object the people.txt CSV file at filePath.
        /// </summary>
        /// <param name="person">The PersonDetails object to be written </param>
        public void WritePersonDetails(PersonDetails person)
        {
            using (var stream = File.Open(Strings.PeopleFileName, FileMode.Append))
            using (var streamWriter = new StreamWriter(stream))
            using (var csvWriter = new CsvWriter(streamWriter, config))
            {
                csvWriter.Context.RegisterClassMap<PeopleDetailsMap>();
                csvWriter.WriteRecord(person);
                csvWriter.NextRecord();
            }

            if(person.Spouse != null)
            {
                using var stream = File.Open(Strings.SpousesFileName, FileMode.Append);
                using var streamWriter = new StreamWriter(stream);
                using var csvWriter = new CsvWriter(streamWriter, config);
                csvWriter.Context.RegisterClassMap<SpouseDetailsMap>();
                csvWriter.WriteRecord(person.Spouse);
                csvWriter.NextRecord();
            }
        }

        public class PeopleDetailsMap : ClassMap<PersonDetails>
        {
            public PeopleDetailsMap()
            {
                Map(m => m.Id).Index(0);
                Map(m => m.FirstName).Index(1);
                Map(m => m.Surname).Index(2);
                Map(m => m.BirthDate).Index(3);
                Map(m => m.MaritalStatus).Index(4);
            }
        }

        public class SpouseDetailsMap : ClassMap<SpouseDetails>
        {
            public SpouseDetailsMap()
            {
                Map(m => m.SpouseID).Index(0);
                Map(m => m.FirstName).Index(1);
                Map(m => m.Surname).Index(2);
                Map(m => m.BirthDate).Index(3);
            }

        }
    }
}
