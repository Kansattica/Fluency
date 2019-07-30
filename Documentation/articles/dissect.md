# Welcome! To a more examined world.

So, last time, we wrote this program:

```cs
Def(Main).I()
```

Not much to look at, but there's a lot going on under the hood. There's two main parts here.

```cs
Def(Main)
```

This guy Def(ines) a function called Main. That is, "Everything connected to this guy is one group called Main." Functions are a big part of working in Fluency, and you'll be seeing plenty of them. In fact, there's another function in our program:

```cs
.I()
```

Not much to look at. This is the simplest Fluency function there is. Every Fluency function, whether it's one you define like Main or one that's built in like I, has the concept of "pipelines", and it's these pipelines that are crucial to reasoning about Fluency and writing effective code. 

Basically, each function takes input from the top pipeline, bottom pipeline, or both, and puts output on the top, bottom, or both. .I() represents the _identity_ function, that is, the function that returns exactly what it takes. This isn't particularly useful on its own- in the future, you'll mostly use it for making sure things line up. If you find .I() hard to remember, .Com() and .Comment() do the same thing. 

Here's a diagram, because I don't like writing big walls of text without something to break it up.

```text
(console) -> Def(Main) -> I() -> (console)
```

I wrote this one myself, but if you find these little graphs helpful, you can generate your own by passing the `--print-graph` flag to Fluency, like this:

```text
dotnet run --print-graph .\coolfile.fl
```

`--print-graph` isn't always a hundred percent correct, but it can help you spot errors in simple programs like this.

The Main function is special because its input and output get hooked up to the console so you can send input to it and write whatever it outputs. 

Putting it all together, you have a program that reads a line from the console, passes it through a function that does nothing, and then prints it out.

Most functions you write in the future won't be Main, but they'll work the same way. Inputs go in, outputs go out. Most of them will even do something more interesting than, uh, nothing. Next up, we'll write a function that does more than nothing.