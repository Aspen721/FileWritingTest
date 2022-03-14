using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileWritingTest
{
    public class PersonDetails
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateOnly BirthDate { get; set; }
        public MaritalStatusEnum MaritalStatus { get; set; }
        public SpouseDetails? Spouse { get; set; }

        public PersonDetails()
        {
            FirstName = "";
            Surname = "";
        }
    }
}
