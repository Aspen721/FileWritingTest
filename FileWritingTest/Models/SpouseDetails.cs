using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWritingTest
{
    public class SpouseDetails
    {
        public int HusbandId { get; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public DateOnly BirthDate { get; set; }

        public SpouseDetails(int HusbandId)
        {
            this.HusbandId = HusbandId;
            FirstName = "";
            Surname = "";
        }
    }
}
