# Fluency
## Hope you like function calls, buddy.


Fluency is a functional programming language inspired by "fluent" APIs like C#'s LINQ. No variables here, only function calls that can branch off each other and merge back in.


### Building and running
Fluency uses .Net Core 2.2 for the console and .Net Standard 2.0 for everything else. I suspect it would work fine with earlier versions, .Net framework, and the like, but I haven't tested. The entire project is self-contained and pure C#. No external packages. To run, simply do:

`dotnet run`

from inside the Console directory. For something more interesting, try:

`dotnet run ../Examples/hello.fl`

And type and enter something after it says "Ready!"

`dotnet run` will eat some command line flags (I know it likes to eat `-h` and `-f`). In that case, simply add two dashes after the `dotnet run` part, like this:

`dotnet run -- ..\Examples\hello.fl -f .\Console.csproj`

or use the long options, like `--help` and `--in-file`.

### Tutorial
Coming soon, but you can look at the examples folder for now!
I find it helps to turn on C#-style syntax highlighting.

### Roadmap
Fluency is currently in a working state, but I'd to do more.
- The design makes it very amenable to being parallelized, so I'd like to do more with that. Currently, each function is lazily evaluated. A parallel Fluency could have each node on its own thread, or even on its own computer, exchanging messages through queues.
- First-class function support
- Real recursion (expand user-defined functions on first request so you can have base cases)
- Better support for users bringing their own C# functions
- Better support for all C# types- at the moment, Fluency just calls them "Any".
- A better tutorial
- Numeric type for ints, floats, decimals, etc.
- Argument support for user defined functions.
