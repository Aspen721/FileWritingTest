using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWritingTest
{
    /// <summary>
    /// The class that handles all input processing from the user and controlls the state machine.
    /// </summary>
    public class InputProcessor
    {
        private readonly List<PersonDetails> personDetailRegistration;
        private readonly StateMachine stateMachine;
        private PersonDetails personDetails;

        /// <summary>
        /// Property for communicating with the UI whether or not spouse fields should be visible
        /// </summary>
        public bool SpouseMode
        {
            get 
            {
                return personDetails.Spouse != null;
            }
            private set
            {
                if (value)
                    personDetails.Spouse = new SpouseDetails(personDetails.Id);
                else
                    personDetails.Spouse = null;

            }
        }

        public InputProcessor()
        {
            personDetailRegistration = PersonDetailsReaderWriter.GetPeopleDetails();
            personDetails = new()
            {
                Id = personDetailRegistration.Count
            };
            stateMachine = new();
        }

        #region State Machine Methods
        //Region is for methods that interact with the state machine.

        /// <summary>
        /// Gets the current state of the state machine.
        /// </summary>
        /// <returns>The current state of the state machine as a StateMachineEnum</returns>
        public State GetCurrentState()
        {
            return stateMachine.CurrentState;
        }

        /// <summary>
        /// Gets the necessary prompt text for the user to know what to input.
        /// </summary>
        /// <returns>The text for the current state's prompt.</returns>
        public string GetStateText()
        {
            return stateMachine.GetStateText();
        }

        /// <summary>
        /// Simply tells the state machine to update.
        /// This is step 5 in the input processing pipeline.
        /// </summary>
        /// <param name="command">The command to send to the state machine.</param>
        private void UpdateStateMachine(Command command)
        {
            stateMachine.Transition(command);
        }
        #endregion

        #region Input Processing Pipeline Methods
        //Region for the methods that process input from the user

        /// <summary>
        /// This method runs the input processing pipeline on a given string input.
        /// 
        /// The steps are:
        /// 1. Validate the string inputed by the user.
        /// 2. Check for a back or restart command (if found, skip to step 5)
        /// 3. Determine the command to send to the state machine
        /// 4. Write appropriate data to the current PersonDetails object
        /// 5. Update the state machine to the next state
        /// </summary>
        /// <param name="input">The string inputed by the user.</param>
        /// <returns>The validated string.</returns>
        public string ProcessInput(string? input)
        {
            string validatedInput = ValidateInput(input);
            Command command = CatchBackOrRestart(validatedInput);
            if (command == Command.Default) //back and restart commands skip the rest of the pipeline
            {
                command = DetermineCommand(validatedInput);
                validatedInput = ConvertAndWriteToObject(validatedInput);
            }
            else
            {
                validatedInput = "";
            }
            UpdateStateMachine(command);
            return validatedInput;
        }

        /// <summary>
        /// This method checks whether the string received is appropriate for the current step of the state machine.
        /// This is step 1 in the input processing pipeline.
        /// </summary>
        /// <param name="userInput">The string inputed by the user.</param>
        /// <returns>The validated string.</returns>
        private string ValidateInput(string? userInput)
        {
            string validatedInput = "";

            switch (stateMachine.CurrentState)
            {
                case State.FirstName:
                case State.Surname:
                case State.SpouseFirstName:
                case State.SpouseSurname:
                    validatedInput = InputValidationMethods.ValidateName(userInput);
                    break;
                case State.BirthDate:
                case State.SpouseBirthDate:
                    validatedInput = InputValidationMethods.ValidateDate(userInput);
                    break;
                case State.Marital:
                    validatedInput = InputValidationMethods.ValidateMaritalStatus(userInput);
                    break;
                case State.Authorize:
                case State.Save:
                case State.Restart:
                    validatedInput = InputValidationMethods.ValidateBoolean(userInput);
                    break;
            }

            return validatedInput;
        }

        /// <summary>
        /// This method checks for back or restart commands.
        /// This is step 2 in the input processing pipeline.
        /// </summary>
        /// <param name="userInput">The string inputed by the user.</param>
        /// <returns>A Back, Restart, or Default Command.</returns>
        private Command CatchBackOrRestart(string input)
        {
            Command returnCommand = Command.Default;

            if (input.Equals("b"))
            {
                if (stateMachine.CurrentState == State.SpouseFirstName)
                    SpouseMode = false;
                returnCommand = Command.Back;
            }
            else if (input.Equals("r"))
            {
                returnCommand = Command.Restart;
            }

            return returnCommand;
        }

        /// <summary>
        /// This method determines what command to send to the state machine to navigate to the next application state.
        /// Step 3 in the input processing pipeline.
        /// </summary>
        /// <param name="input">The validated input string from the user for navigation purposes.</param>
        /// <returns>The determined Command.</returns>
        private Command DetermineCommand(string input)
        {
            Command returnCommand = Command.Default;

            switch (stateMachine.CurrentState)
            {
                case State.FirstName:
                    returnCommand = Command.Continue;
                    break;

                case State.Surname:
                    returnCommand = Command.Continue;
                    break;

                case State.BirthDate:
                    DateOnly birthday = ConversionMethods.ConvertDate(input);
                    if (IsUserTooYoung(birthday))
                        returnCommand = Command.Deny;
                    else if (IsAgeVerificationNecessary(birthday))
                        returnCommand = Command.Authorize;
                    else
                        returnCommand = Command.Continue;
                    break;

                case State.Authorize:
                    if (!ConversionMethods.ConvertYesNo(input))
                        returnCommand = Command.Deny;
                    else
                        returnCommand = Command.Continue;
                    break;

                case State.Marital:
                    MaritalStatusEnum ms = ConversionMethods.ConvertMaritalStatus(input);
                    if (!IsSpouseInputNecessary(ms))
                    {
                        returnCommand = Command.Continue;
                    }
                    else
                    {
                        SpouseMode = true;
                        returnCommand = Command.SpouseMode;
                    }
                    break;

                case State.SpouseFirstName:
                    returnCommand = Command.Continue;
                    break;

                case State.SpouseSurname:
                    returnCommand = Command.Continue;
                    break;

                case State.SpouseBirthDate:
                    returnCommand = Command.Continue;
                    break;

                case State.Save:
                    if (!ConversionMethods.ConvertYesNo(input))
                    {
                        if (personDetails.Spouse != null)
                            returnCommand = Command.SpouseBack;
                        else
                            returnCommand = Command.Back;
                    }
                    else
                    {
                        returnCommand = Command.Continue;
                    }
                    break;

                case State.ConfirmRestart:
                    if (!ConversionMethods.ConvertYesNo(input))
                        returnCommand = Command.Back;
                    else
                        returnCommand = Command.Continue;
                    break;
            }

            return returnCommand;
        }

        /// <summary>
        /// This method determines which field in the PersonDetails object to write to. 
        /// This is step 4 in the input processing pipeline.
        /// 
        /// Note this method will also convert the input string to its proper representation in the UI.
        /// </summary>
        /// <param name="input">The string that contains the data to write to the PersonDetails object.</param>
        private string ConvertAndWriteToObject(string input)
        {
            switch (stateMachine.CurrentState)
            {
                case State.FirstName:
                    personDetails.FirstName = input;
                    break;

                case State.Surname:
                    personDetails.Surname = input;
                    break;

                case State.BirthDate:
                    personDetails.BirthDate = ConversionMethods.ConvertDate(input);
                    input = personDetails.BirthDate.ToString();
                    break;

                case State.Marital:
                    personDetails.MaritalStatus = ConversionMethods.ConvertMaritalStatus(input);
                    string? possibleNull = Enum.GetName(personDetails.MaritalStatus);
                    if (possibleNull != null)
                        input = possibleNull;
                    break;

                case State.SpouseFirstName:
                    if (personDetails.Spouse != null)
                        personDetails.Spouse.FirstName = input;
                    break;

                case State.SpouseSurname:
                    if (personDetails.Spouse != null)
                        personDetails.Spouse.Surname = input;
                    break;

                case State.SpouseBirthDate:
                    if (personDetails.Spouse != null)
                    {
                        personDetails.Spouse.BirthDate = ConversionMethods.ConvertDate(input);
                        input = personDetails.Spouse.BirthDate.ToString();
                    }
                    break;
            }

            return input;
        }

        /// <summary>
        /// Restarts the state machine, saves the inputed PersonDetails if necessary, and creates a new PersonDetails.
        /// </summary>
        public void ReturnToStart()
        {
            switch (stateMachine.CurrentState)
            {
                case State.Confirmed:
                    personDetailRegistration.Add(personDetails);
                    PersonDetailsReaderWriter.WritePersonDetails(personDetails);
                    stateMachine.Transition(Command.Continue);
                    break;

                case State.Denied:
                case State.Restart:
                    stateMachine.Transition(Command.Continue);
                    break;
            }

            personDetails = new();
        }
        #endregion

        #region State Machine Branching Methods
        //Region is for methods controlling the navigation of the state machine in the DetermindCommand method

        /// <summary>
        /// Determines whether or not the application needs to gather spouse data for a given MaritalStatusEnum
        /// </summary>
        /// <param name="maritalStatus">The marital status in question.</param>
        /// <returns>False if maritalStatus is Single, Divorced, or Widowed. True otherwise.</returns>
        private static bool IsSpouseInputNecessary(MaritalStatusEnum maritalStatus)
        {
            return maritalStatus switch
            {
                MaritalStatusEnum.Single or MaritalStatusEnum.Divorced or MaritalStatusEnum.Widowed => false,
                _ => true,
            };
        }

        /// <summary>
        /// Determines whether a user is too young to register with the application.
        /// </summary>
        /// <param name="birthDay">The date of birth of the user.</param>
        /// <returns>True if the user is under the age of 16. False otherwise.</returns>
        private static bool IsUserTooYoung(DateOnly birthDay)
        {
            if (ConversionMethods.CalculateAge(birthDay) >= 16)
                return false;
            return true;
        }

        /// <summary>
        /// Determines whether a user needs parental verification to register with the application.
        /// </summary>
        /// <param name="birthDay">The date of birth of the user.</param>
        /// <returns>True if the user is under the age of 18. False otherwise.</returns>
        private static bool IsAgeVerificationNecessary(DateOnly birthDay)
        {
            if (ConversionMethods.CalculateAge(birthDay) >= 18)
                return false;
            return true;
        }
        #endregion
    }
}
