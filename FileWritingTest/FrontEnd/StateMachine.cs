using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWritingTest
{
    /// <summary>
    /// Enum representing possible application states
    /// </summary>
    public enum State
    {
        FirstName,
        Surname,
        BirthDate,
        Authorize,
        Marital,
        SpouseFirstName,
        SpouseSurname,
        SpouseBirthDate,
        Save,
        Confirmed,
        Denied,
        ConfirmRestart,
        Restart
    }

    /// <summary>
    /// Enum representing commands to send to the state machine to change application state
    /// </summary>
    public enum Command
    {
        Back,
        Continue,
        SpouseMode,
        SpouseBack,
        Authorize,
        Deny,
        Restart,
        Default
    }

    /// <summary>
    /// The StateMachine class has three purposes:
    /// 
    /// 1. Track the state of user input
    /// 2. Deliver appropriate prompts to the UI
    /// 3. Tell what commands are valid for a given state
    /// 
    /// The first is tracked via the CurrentState property
    /// The second is accomplished through the transitionsDictionary field
    /// The thrid is accomplished through the stateInfoDictionary field
    /// </summary>
    public class StateMachine
    {
        #region Fields
        private readonly Dictionary<StateTransition, State> transitionsDictionary;
        private readonly Dictionary<State, StateInfo> stateInfoDictionary;
        private State previousState;
        #endregion

        #region Properties
        public State CurrentState { get; private set; }
        #endregion

        #region Constructor
        public StateMachine()
        {
            CurrentState = State.FirstName;

            //The state machine represented as a dictionary of transitions to different states
            transitionsDictionary = new Dictionary<StateTransition, State>
            {
                { new StateTransition(State.FirstName, Command.Continue), State.Surname },
                { new StateTransition(State.FirstName, Command.Back), State.FirstName },
                { new StateTransition(State.Surname, Command.Continue), State.BirthDate },
                { new StateTransition(State.Surname, Command.Back), State.FirstName },
                //Age Verification Start
                { new StateTransition(State.BirthDate, Command.Authorize), State.Authorize },
                { new StateTransition(State.Authorize, Command.Continue), State.Marital },
                { new StateTransition(State.Authorize, Command.Deny), State.Denied },
                { new StateTransition(State.BirthDate, Command.Deny), State.Denied },
                //Age Verification End
                { new StateTransition(State.BirthDate, Command.Continue), State.Marital },
                { new StateTransition(State.BirthDate, Command.Back), State.Surname },
                //SpouseMode Start
                { new StateTransition(State.Marital, Command.SpouseMode), State.SpouseFirstName },
                { new StateTransition(State.SpouseFirstName, Command.Continue), State.SpouseSurname },
                { new StateTransition(State.SpouseFirstName, Command.Back), State.Marital },
                { new StateTransition(State.SpouseSurname, Command.Continue), State.SpouseBirthDate },
                { new StateTransition(State.SpouseSurname, Command.Back), State.SpouseFirstName },
                { new StateTransition(State.SpouseBirthDate, Command.Continue), State.Save },
                { new StateTransition(State.SpouseBirthDate, Command.Back), State.SpouseSurname },
                //SpouseMode End
                { new StateTransition(State.Marital, Command.Continue), State.Save },
                { new StateTransition(State.Marital, Command.Back), State.BirthDate },
                { new StateTransition(State.Save, Command.Continue), State.Confirmed },
                { new StateTransition(State.Save, Command.SpouseBack), State.SpouseBirthDate },
                { new StateTransition(State.Save, Command.Back), State.Marital },
                //Restart Transitions Start
                { new StateTransition(State.FirstName, Command.Restart), State.ConfirmRestart },
                { new StateTransition(State.Surname, Command.Restart), State.ConfirmRestart },
                { new StateTransition(State.BirthDate, Command.Restart), State.ConfirmRestart },
                { new StateTransition(State.Marital, Command.Restart), State.ConfirmRestart },
                { new StateTransition(State.SpouseFirstName, Command.Restart), State.ConfirmRestart },
                { new StateTransition(State.SpouseSurname, Command.Restart), State.ConfirmRestart },
                { new StateTransition(State.SpouseBirthDate, Command.Restart), State.ConfirmRestart },
                { new StateTransition(State.ConfirmRestart, Command.Continue), State.Restart },
                //Restart Transitions End
                { new StateTransition(State.Confirmed, Command.Continue), State.FirstName }, //no inputs allowed here
                { new StateTransition(State.Denied, Command.Continue), State.FirstName }, //or here
                { new StateTransition(State.Restart, Command.Continue), State.FirstName } //or here
            };

            //A dictionary that contains all the prompt strings for each state
            stateInfoDictionary = new Dictionary<State, StateInfo>
            {
                { State.FirstName, new StateInfo(Strings.FirstName.Prompt, Strings.FirstName.Info) },
                { State.Surname, new StateInfo(Strings.Surname.Prompt, Strings.Surname.Info) },
                { State.BirthDate, new StateInfo(Strings.BirthDate.Prompt, Strings.BirthDate.Info) },
                { State.Authorize, new StateInfo(Strings.Authorize.Prompt, Strings.Authorize.Info) },
                { State.Marital, new StateInfo(Strings.Marital.Prompt, Strings.Marital.Info) },
                { State.SpouseFirstName, new StateInfo(Strings.SpouseFirstName.Prompt, Strings.SpouseFirstName.Info) },
                { State.SpouseSurname, new StateInfo(Strings.SpouseSurname.Prompt, Strings.SpouseSurname.Info) },
                { State.SpouseBirthDate, new StateInfo(Strings.SpouseBirthDate.Prompt, Strings.SpouseBirthDate.Info) },
                { State.Save, new StateInfo(Strings.Save.Prompt, Strings.Save.Info) },
                { State.Confirmed, new StateInfo(Strings.Confirmed.Prompt, Strings.Confirmed.Info) },
                { State.Denied, new StateInfo(Strings.Denied.Prompt, Strings.Denied.Info) },
                { State.ConfirmRestart, new StateInfo(Strings.Restart.Prompt, Strings.Restart.Info) }
            };
        }
        #endregion

        #region Methods
        //Methods for navigating the state machine or getting info on the current state of it

        /// <summary>
        /// Method for transitioning the state machine to the next state based on a given command.
        /// </summary>
        /// <param name="command">The Command to give the state machine.</param>
        /// <returns>The new State of the state machine.</returns>
        /// <exception cref="InvalidOperationException">Thrown if transition leads to an invalid application state.</exception>
        public State Transition(Command command)
        {
            StateTransition transition = new(CurrentState, command);
            if (CurrentState == State.ConfirmRestart && command == Command.Back)
                CurrentState = previousState;
            else
            {
                if (!transitionsDictionary.TryGetValue(transition, out State nextState))
                    throw new InvalidOperationException("Invalid transition: " + CurrentState + " -> " + command);
                previousState = CurrentState;
                CurrentState = nextState;
            }
            return CurrentState;
        }

        /// <summary>
        /// Method for getting the user prompt associated with the current state of the state machine.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception">Thrown if the current application state does not have an associated StateInfo</exception>
        public string GetStateText()
        {
            StringBuilder sb = new();
            if (!stateInfoDictionary.TryGetValue(CurrentState, out StateInfo? strings))
                throw new Exception("State does not have state info.");
            sb.AppendLine(strings.prompt);
            sb.AppendLine(strings.info);
            return sb.ToString();
        }
        #endregion

        #region Classes
        //Classes that contain info on the state machine and its navigation

        /// <summary>
        /// A Key Value Pair for the current state and the command.
        /// This KVP is paired with a valid State value in the transitionsDictionary
        /// </summary>
        class StateTransition
        {
            readonly State currentState;
            readonly Command command;

            public StateTransition(State currentState, Command command)
            {
                this.currentState = currentState;
                this.command = command;
            }

            public override int GetHashCode()
            {
                return 17 + 31 * currentState.GetHashCode() + 31 * command.GetHashCode();
            }

            public override bool Equals(object? obj)
            {
                return obj is StateTransition other && this.currentState == other.currentState && this.command == other.command;
            }
        }

        /// <summary>
        /// A class for storing info about a current state
        /// Each one created is associated with a State in the stateInfoDictionary
        /// </summary>
        public class StateInfo
        {
            public readonly string prompt;
            public readonly string info;
            public StateInfo(string prompt, string info)
            {
                this.prompt = prompt;
                this.info = info;
            }
        }
        #endregion
    }
}
