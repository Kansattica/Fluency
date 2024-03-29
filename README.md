# Fluency
## Hope you like function calls, buddy.

Fluency is a functional programming language inspired by "fluent" APIs like C#'s LINQ. No variables here, only function calls that can branch off each other and merge back in.

### You can try Fluency right now in your browser! <https://kansattica.github.io/FluencyOnline/>
Note that Fluency Online is still in beta. If something doesn't work, please let me know and try the download and run version.

### Building and running

If you simply want to download and run Fluency, you can do it from the [Releases page](https://github.com/Kansattica/Fluency/releases)!

Simply download the correct binary for your system and run it from the command line.

If you want to build it yourself and mess with the code, the instructions are below.

Fluency uses .Net Core 3.1 for the console and .Net Standard 2.1 for everything else. I suspect it would work fine with earlier versions, .Net framework, and the like with some tweaking, but I haven't tested. The entire project is self-contained and pure C#. No external packages. To run, simply do:

`dotnet run`

from inside the Console directory. For something more interesting, try:

`dotnet run ../Examples/hello.fl`

And type and enter something after it says "Ready!"

`dotnet run` will eat some command line flags (I know it likes to eat `-h`). In that case, simply add two dashes after the `dotnet run` part, like this:

`dotnet run -- ..\Examples\hello.fl -h`

or use the long options, like `--help` and `--in-file`.

If you're doing anything that involves a lot of calls (and everything involves a lot of calls), consider passing the --configuration release argument to dotnet run, like so:

`dotnet run --configuration release -- ..\Examples\prime.fl`

This appears to turn on tail call optimization and generally make the code run a lot faster.

### Tutorial
- I have the beginnings of a tutorial at <https://kansattica.github.io/Fluency/articles/intro.html>!
- prime.fl probably has the most complete program and best example of Fluency if you'd like to start that way.
- Code documentation is available at <https://kansattica.github.io/Fluency>.

### Roadmap
Fluency is currently in a working state, but I'd love to do more.
- [ ] The design makes it very amenable to being parallelized, so I'd like to do more with that. Currently, each function is lazily evaluated. A parallel Fluency could have each node on its own thread, or even on its own computer, exchanging messages through queues.
- [ ] First-class function support.
- [X] Real recursion (expand user-defined functions on first request so you can have base cases)
- [ ] Better support for users bringing their own C# functions.
- [ ] Better support for all C# types- at the moment, Fluency just calls them "Any".
- [X] Any tutorial.
- [ ] A good tutorial.
- [ ] Numeric type for ints, floats, decimals, etc.
- [ ] Function overloading, at least for things like Add.
- [X] Argument support for user defined functions.
- [X] Top and bottom arguments.
- [ ] Graph printer is a hack and doesn't do all graphs correctly.
- [ ] Add support for graphs with nodes that don't branch off from the head, such as Const.
- [X] Add support for graphs with heads that don't go straight across the top
- [ ] Runtime support for queue-type nodes so they don't have to maintain their own state.
- [X] [A website where you can enter Fluency code and have it executed without building the code yourself!](https://kansattica.github.io/FluencyOnline)
- [ ] Optimizations:
    - [ ] Easy ones like "remove comments".
    - [ ] Let nodes signal that they're done and can be removed 
        - [ ] If a node is in a state where it's just passing values from A to B, it can be removed and the runtime can stitch things up. That is, if the node B in A -> B -> C knows that all it's doing is passing from A to C (perhaps because it's a MergeBottom that's already done the bottom, or it's a comment, or it's finished and won't be passing anything any more), we can remove the node and save some time and a layer in the call stack.
        - [ ] One big limit on how much computation can be done is the call stack, so anything that can help reduce that is good, especially with recursive calls.
        - [X] Finished function pruning is done for user-defined functions. They get re-expanded and replaced in place.
