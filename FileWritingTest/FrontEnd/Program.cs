using FileWritingTest;
using System.Text;

#region Global Variables
string? userInput;
string errorMessage = "";
StringBuilder currentPrompt = new();
InputProcessor inputProcessor = new();
Config config = Config.Instance;
Dictionary<State, Coordinates?> fieldCoordinates = new()
{
    { State.FirstName, new Coordinates(20, 6) },
    { State.Surname, new Coordinates(20, 7) },
    { State.BirthDate, new Coordinates(20, 8) },
    { State.Marital, new Coordinates(20, 9) },
    { State.SpouseFirstName, new Coordinates(75, 6) },
    { State.SpouseSurname, new Coordinates(75, 7) },
    { State.SpouseBirthDate, new Coordinates(75, 8) }
}; //TODO: make coordinates relational
#endregion

//Operation Loop
do
{
    #region Header
    Console.WriteLine("Welcome to the Escher Customer Records Input Application (ECRIA)");
    Console.WriteLine("At any point in the process, hold ctrl and press c to quit.");
    Console.WriteLine("Enter \"b\" to move backwards in operation and re-enter info.");
    Console.WriteLine("Enter \"r\" to restart.\n");
    Console.WriteLine();
    Console.WriteLine("First Name:");
    Console.WriteLine("Surname:");
    Console.WriteLine("Date of Birth:");
    Console.WriteLine("Marital Status:");
    Console.WriteLine();
    Console.WriteLine();
    #endregion

    //Data gathering loop
    while (inputProcessor.GetCurrentState() != State.Confirmed &&
           inputProcessor.GetCurrentState() != State.Denied &&
           inputProcessor.GetCurrentState() != State.Restart)
    {
        int startTop = Console.CursorTop;

        //Write prompt to console
        currentPrompt.Clear();
        currentPrompt.AppendLine(inputProcessor.GetStateText());
        if(inputProcessor.GetCurrentState() == State.Marital)
        {
            currentPrompt.AppendLine(config.PrintMaritalStatuses());
            currentPrompt.AppendLine("");
        }
        currentPrompt.AppendLine(errorMessage);
        currentPrompt.Append('>');
        Console.Write(currentPrompt.ToString());

        //Gather input
        userInput = Console.ReadLine();

        //Process input
        try
        {
            State currentState = inputProcessor.GetCurrentState();
            string validatedInput = inputProcessor.ProcessInput(userInput);
            WriteToField(validatedInput, currentState);
            if (inputProcessor.SpouseMode)
                WriteSpouseFields();
            else
                EraseSpouseFields();
            errorMessage = "";
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }

        //Erase prompt
        FlushPrompt(startTop);
    }

    //Write to file, print confirmation, and reset application
    if (inputProcessor.GetCurrentState() != State.Restart)
    {
        currentPrompt.Clear();
        currentPrompt.Append(inputProcessor.GetStateText());
        Console.WriteLine(currentPrompt.ToString());
        for (int i = 5; i > 0; i--)
        {
            Console.WriteLine("..." + i);
            Thread.Sleep(1000);
        }
    }
    inputProcessor.ReturnToStart();
    Console.Clear();

} while (true);

#region UI Methods And Classes
//All the methods that have to do with updating the state of the UI

/// <summary>
/// Erase the current prompt so that a new one can take its place
/// </summary>
void FlushPrompt(int startTop)
{
    while (Console.CursorTop > startTop)
    {
        int cursorTop = Console.CursorTop;
        Console.CursorTop = cursorTop - 1;
        Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");
    }
}

/// <summary>
/// Create the UI "assets" for showing the current spouse input
/// </summary>
void WriteSpouseFields()
{
    (int currentLeft, int currentTop) = Console.GetCursorPosition();
    Console.SetCursorPosition(50, 6);
    Console.Write("Spouse's First Name:");
    Console.SetCursorPosition(50, 7);
    Console.Write("Spouse's Surname:");
    Console.SetCursorPosition(50, 8);
    Console.Write("Spouse's Date of Birth:");
    Console.SetCursorPosition(currentLeft, currentTop);
}

/// <summary>
/// Erase the spouse related UI "assets"
/// </summary>
void EraseSpouseFields()
{
    (int currentLeft, int currentTop) = Console.GetCursorPosition();
    Console.SetCursorPosition(50, 6);
    Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft));
    Console.SetCursorPosition(50, 7);
    Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft));
    Console.SetCursorPosition(50, 8);
    Console.Write(new string(' ', Console.WindowWidth - Console.CursorLeft));
    Console.SetCursorPosition(currentLeft, currentTop);
}

/// <summary>
/// Write to a field on the UI based on the current application state
/// </summary>
void WriteToField(string userInput, State programState)
{
    fieldCoordinates.TryGetValue(programState, out global::Coordinates? fieldXY);
    if (fieldXY != null)
    {
        (int currentLeft, int currentTop) = Console.GetCursorPosition();
        Console.SetCursorPosition(fieldXY.X, fieldXY.Y);
        Console.Write(userInput);
        Console.Write(new String(' ', config.Name_MaxLength));
        Console.SetCursorPosition(currentLeft, currentTop);
    }
}

/// <summary>
/// Class for saving the coordinates of UI "assets"
/// </summary>
class Coordinates
{
    public int X { get; }
    public int Y { get; }

    public Coordinates(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public (int x, int y) GetCoordinates()
    {
        return (X, Y);
    }
}
#endregion