using System.Collections.Generic;

namespace FileWritingTest
{
    public interface IFileReaderWriter
    {
        //retrieve PersonDetails from file
        public List<PersonDetails> GetPeopleDetails();

        //write PersonDetails to file
        public void WritePersonDetails(PersonDetails person);
    }
}