# EscherAssignment
 This project is intended to be a showcase of various things I can do with C# .NET. It takes various bits of data from the user and saves the information into a CSV file `people.txt` delimited by `|` characters. It will also read from this file to ensure that the details of each person entered are matched with a unique ID. Spouse information can also be entered if the person indicates that they are married. This information is written to `spouse.txt`. Both of these files are saved in the 'Records' folder in the directory where the exe is run.

 In order to accomplish this, information is gathered incrementally through the CLI. This information is written to the fields of a `PersonDetails` object and a `SpouseDetails` object if necessary, then this object is written to a file through the use of CSVHelper. That library can be found here: https://joshclose.github.io/CsvHelper/

The `PersonDetails` and `SpouseDetails` objects were written to be as simple as possible for versatility purposes. The details of each are as follows.


## Models
### PersonDetails

Each `PersonDetails` object has the following properties.

`int ID`
`string FirstName`
`string Surname`
`DateOnly BirthDate`
`MaritalStatusEnum MaritalStatus`
`SpouseDetails? Spouse`

The Spouse property can be null if the `MaritalStatus` indicates that the person is `Single`, `Divorced`, or `Widowed`. The Spouse property cannot be null if the MaritalStatus property indicates that the person is `Married`, `Separated`, or `Partnered` (indicating a domestic partnership). These are all the values of the MaritalStatusEnum, but these values can be changed in `config.txt`. More on that below.

### SpouseDetails

Very similar to the `PersonDetails` class, the properties of `SpouseDetails` are as follows.

`int SpouseID`
`string FirstName`
`string Surname`
`DateOnly BirthDate`

The SpouseID is always the same as the unique ID given to its associated PersonDetails object, meaning each PersonDetails object can only have one associated SpouseDetails object and vice versa.

### people.txt and spouse.txt

The objects are written to their associated files in this format:

people.txt: `ID|FirstName|Surname|BirthDate|MaritalStatus`
Example:    `0|Torin|List|07/21/1993|Married`

people.txt: `SpouseID|FirstName|Surname|BirthDate`
Example:    `0|My|Cat|09/21/1999`

You can find these files in the Records folder after the application is run.


## Data Gathering Procedure

### InputProcessor

The data gathering is split into 5 different steps in the `InputProcessor` class. The program first initializes an `InputProcessor` object that delivers the prompt for the first state of the program, gathering the users first name. Once the input is gathered, the `InputProcessor` processes the data in 5 separate steps that are as follows.

1. Validate that the input string is valid for its particular field.
2. Catch a `Back` or `Restart` command (the user entered `"b"` or `"r"` respectively) to see if further processing is actually needed. If not skip to step 5.
3. Determine what the next state of the state machine is. More on that below.
4. Convert the input string to its proper code representation and write it to its associated property in the current PersonDetails object and associated SpouseDetails object if necessary.
5. Update the state machine to the next state of input gathering determined from either step 2 or 3. For example, if the state machine is in the `FirstName` state and given a `Continue` command, it will update to the `Surname` state. If the state machine is in the `Surname` state and given a `Back` command, it will update to the `FirstName` state.

The InputProcessor then delivers the prompt for the next state of input gathering, gradually navigating the state machine until it reaches the `Confirmed`, `Denied`, or `Restart`

### StateMachine

The `StateMachine` class holds the current state of data gathering in a dictionary of transitions and states. For each State, there is an associated Command and another State that the state machine navigates to based on a given command. The possible States and Commands are as follows.

States:

`FirstName`,
`Surname`,
`BirthDate`,
`Authorize`,
`Marital`,
`SpouseFirstName`,
`SpouseSurname`,
`SpouseBirthDate`,
`Save`,
`Confirmed`,
`Denied`,
`ConfirmRestart`,
`Restart`

Commands:

`Back`,
`Continue`,
`SpouseMode`,
`SpouseBack`,
`Authorize`,
`Deny`,
`Restart`,
`Default`

The state machine and all its states and commands can be visualized in this diagram (with restart system removed): https://i.imgur.com/d2TPBVP.png

The state machine ensures that if a user inputed data incorrectly and it was accepted by the system, they can go back and redo that field at any point in the process. It also ensures that content delivery to and from the UI is orderly and deterministic regardless of what UI framework is built on top of it. This was by far the most time consuming part of the whole application, so please like it or I will be sad :(

### PersonDetailsReaderWriter

If all goes well and the state machine reaches the Confirmed state, the current `PersonDetails` object in `InputProcessor` will be saved to `Records/people.txt` through the static class `PersonDetailsReaderWriter`.
The `PersonDetailsReaderWriter` class is meant to be an interface of sorts with the "back end" or code that writes the PersonDetails object to a file or server database. In this case it simply finds the appropriate file type to save the PersonDetails object to, in this case a CSV file, and saves all the fields there in the format specified above.

However, the `PersonDetailsReaderWriter` is designed to detect the filetype of the people and spouse files through their extensions so it can possibly write in json or xml format. The `PersonDetails` object is also designed to be perfect for ORM or object relational mapping with a database. One needs only to set up the proper scaffolding. Perhaps with ASP.NET MVC and entity framework?

## Future Design

If I had more time I would implement:

-More and better unit tests. I am still very new to TDD and NUnit and it shows. I'm willing to put in the effort to learn though because I like the idea a lot.
-JSON and XML support, as these are the two most popular ways of persisting files and formating objects to send to a server
-Database support for keeping all this info in a more logical format that runs 24/7

Plus I would refactor things just a bit. But hopefully it's organized and readable enough as it is!