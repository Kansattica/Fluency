# Welcome to Fluency!
## Hope you like function calls, buddy.

Fluency is a functional programming language inspired by "fluent" APIs like C#'s LINQ. No variables here, only function calls that can branch off each other and merge back in.

You can find the code on [my github](https://github.com/Kansattica/Fluency).

### Building and running
Fluency uses .Net Core 2.2 for the console and .Net Standard 2.0 for everything else. I suspect it would work fine with earlier versions, .Net framework, and the like, but I haven't tested. The entire project is self-contained and pure C#. No external packages. To run, simply do:

`dotnet run`

from inside the Console directory. For something more interesting, try:

`dotnet run ../Examples/hello.fl`

And type and enter something after it says "Ready!"

`dotnet run` will eat some command line flags (I know it likes to eat `-h` and `-f`). In that case, simply add two dashes after the `dotnet run` part, like this:

`dotnet run -- ..\Examples\hello.fl -f .\Console.csproj`

or use the long options, like `--help` and `--in-file`.

If you're doing anything that involves a lot of calls (and everything involves a lot of calls), consider passing the --configuration release argument to dotnet run, like so:

`dotnet run --configuration release -- ..\Examples\prime.fl`

This appears to turn on tail call optimization and generally make the code run a lot faster.

A real tutorial is coming, but for now, feel free to check out the list of [builtin functions](api/Fluency.Execution.Functions.BuiltIn.html) and the [examples](https://github.com/Kansattica/Fluency/tree/master/Examples), especially [prime.fl](https://github.com/Kansattica/Fluency/blob/master/Examples/prime.fl).