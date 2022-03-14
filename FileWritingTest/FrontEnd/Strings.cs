using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWritingTest
{
    public static class Strings
    {
        public const string DataDirectory = "Records";
        public const string PeopleFileName = DataDirectory + "/people.txt";
        public const string SpousesFileName = DataDirectory + "/spouses.txt";
        public static class FirstName
        {
            public const string Prompt = "Please enter your FIRST NAME.";
            public const string Info = "Note that only English characters are supported at this time.";
        }
        public static class Surname
        {
            public const string Prompt = "Please enter your LAST NAME.";
            public const string Info = "Note that only English characters are supported at this time.";
        }
        public static class BirthDate
        {
            public const string Prompt = "Please enter your DATE OF BIRTH.";
            public const string Info = "The format should be mm/dd/yyyy. Example: 07/21/1993";
        }
        public static class Authorize
        {
            public const string Prompt = "You need your parent or guardian's permission to register.";
            public const string Info = "Have you gotten permission from them to register for this service? (y/n)";
        }
        public static class Marital
        {
            public const string Prompt = "Please enter your MARITAL STATUS based on the choices below.";
            public const string Info = "Enter the number that corresponds with your status.";
        }
        public static class SpouseFirstName
        {
            public const string Prompt = "Please enter your SPOUSE'S FIRST NAME.";
            public const string Info = "Note that only English characters are supported at this time.";
        }
        public static class SpouseSurname
        {
            public const string Prompt = "Please enter your SPOUSE'S LAST NAME.";
            public const string Info = "Note that only English characters are supported at this time.";
        }
        public static class SpouseBirthDate
        {
            public const string Prompt = "Please enter your SPOUSE'S DATE OF BIRTH.";
            public const string Info = "The format should be mm/dd/yyyy. Example: 07/21/1993";
        }
        public static class Save
        {
            public const string Prompt = "Please review your entries above.";
            public const string Info = "Do you wish to submit and finalize your details? (y/n)";
        }
        public static class Confirmed
        {
            public const string Prompt = "Thank you! Your information has been saved.";
            public const string Info = "Please make space for the next person in line. This message will time out in: ";
        }
        public static class Denied
        {
            public const string Prompt = "You are too young to register for this service.";
            public const string Info = "Please uhh... come back with your mom. This message will time out in: ";
        }
        public static class Restart
        {
            public const string Prompt = "Are you sure you'd like to undo all changes and restart?";
            public const string Info = "Enter y to confirm and n to cancel.";
        }
    }
}
