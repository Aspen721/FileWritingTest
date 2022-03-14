using NUnit.Framework;
using FileWritingTest;
using System;

namespace FileWritingTestTests
{
    public class InputValidationMethodTests
    {
        [Test]
        public void String_Cannot_Be_Null_Empty_Or_Whitespace()
        {
            //null
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(null));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(null));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(null));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateBoolean(null));
            //empty
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(""));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(""));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(""));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateBoolean(""));
            //whitespace
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(" "));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(" "));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(" "));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateBoolean(" "));
        }

        [Test]
        public void Back_Allowed_On_All_Except_Boolean()
        {
            string input = "b";

            Assert.AreEqual(input, InputValidationMethods.ValidateName(input));
            Assert.AreEqual(input, InputValidationMethods.ValidateDate(input));
            Assert.AreEqual(input, InputValidationMethods.ValidateMaritalStatus(input));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateBoolean(input));
        }

        [Test]
        public void Return_Allowed_On_All_Except_Boolean()
        {
            string input = "r";

            Assert.AreEqual(input, InputValidationMethods.ValidateName(input));
            Assert.AreEqual(input, InputValidationMethods.ValidateDate(input));
            Assert.AreEqual(input, InputValidationMethods.ValidateMaritalStatus(input));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateBoolean(input));
        }

        [Test]
        public void Name_Length_Cannot_Be_Greater_Than_Config_Max()
        {
            Config config = Config.Instance;
            string pass = new('a', config.Name_MaxLength);
            string fail = new('a', config.Name_MaxLength + 1);

            Assert.AreEqual(pass, InputValidationMethods.ValidateName(pass));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(fail));
        }

        [Test]
        public void Name_Length_Cannot_Be_Less_Than_Config_Min()
        {
            Config config = Config.Instance;
            string pass = new('a', config.Name_MinLength);
            string fail = new('a', config.Name_MinLength - 1);

            Assert.AreEqual(pass, InputValidationMethods.ValidateName(pass));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(fail));
        }

        [Test]
        public void Name_Must_Contain_Only_Letters()
        {
            string numbers = "andrew1";
            string whitespace = "and rew";
            string symbols = "and@rew";

            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(numbers));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(whitespace));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateName(symbols));
        }

        [Test]
        public void Date_Must_Be_In_Valid_Format()
        {
            string wrong1 = "2003-04-21";
            string wrong2 = "21/04/2005";
            string wrong3 = "09-21-2001";
            string correct = "07/21/1993";

            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(wrong1));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(wrong2));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(wrong3));
            Assert.AreEqual(correct, InputValidationMethods.ValidateDate(correct));
        }

        [Test]
        public void Date_Must_Not_Be_In_Future()
        {
            DateTime now = DateTime.Now;
            DateTime future = DateTime.Now.AddDays(1);

            string futureString = DateOnly.FromDateTime(future).ToString("MM/dd/yyyy");
            string nowString = DateOnly.FromDateTime(now).ToString("MM/dd/yyyy");

            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(futureString));
            Assert.AreEqual(nowString, InputValidationMethods.ValidateDate(nowString));
        }

        [Test]
        public void Date_Must_Not_Be_Greater_Than_Age_Max()
        {
            DateTime maxAge = DateTime.Now.AddYears(-Config.Instance.Age_Max);
            DateTime higherThanMaxAge = DateTime.Now.AddYears((-Config.Instance.Age_Max) - 1);

            string maxAgeString = DateOnly.FromDateTime(maxAge).ToString("MM/dd/yyyy");
            string higherThanMaxAgeString = DateOnly.FromDateTime(higherThanMaxAge).ToString("MM/dd/yyyy");

            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateDate(higherThanMaxAgeString));
            Assert.AreEqual(maxAgeString, InputValidationMethods.ValidateDate(maxAgeString));
        }

        [Test]
        public void Marital_Status_Input_Must_Be_Number()
        {
            string numbers = "andrew";
            string whitespace = " ";
            string symbols = "@";

            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(numbers));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(whitespace));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(symbols));
        }

        [Test]
        public void Marital_Status_Input_Must_Match_Array_Index()
        {
            Config config = Config.Instance;

            //indeces are offset by +1 for the user
            int num1 = 0;
            int num2 = config.MaritalStatusArray.Length + 1;
            string input1 = num1.ToString();
            string input2 = num2.ToString();

            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(input1));
            Assert.Throws<ArgumentException>(() => InputValidationMethods.ValidateMaritalStatus(input2));
            for(; num1 >= num2; num1++)
                Assert.AreEqual(num1.ToString(), InputValidationMethods.ValidateMaritalStatus(num1.ToString()));
        }
    }
}