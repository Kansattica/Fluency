# Welcome! To a more useful world.

Alright, we've gotten the simple example out of the way. Now let's do something real: add two numbers together. 

```cs
Def(Main).ParseInt().Dup().Com().Add()
                        \.Com("n")./
```

I copied this out of `doubler.fl` in the Examples folder. There's a little more going on this time, but it's not too bad. Try and see if you can figure out what this one does before running it or reading on.

---

If you guessed "It reads a number and doubles it", you would be correct! Here's a sample run I did:

```text
...\Fluency\Console> dotnet run --print-graph ..\Examples\doubler.fl
Def(Main) -> ParseInt() -> Dup() -> Com() -> Add() -> Return (top)
                               \ Com("n")    /

Ready!

100
200
200
400
500
1000
700
1400
888
1776
-2
-4
0
0
```

You've seen `Def(Main)` before- here it simply marks where our program starts and that it outputs to the console when done.


`ParseInt()` is a new one. See, everything that comes in from the console is a string- simple text. `ParseInt()` says "Turn this into a number, please. I promise it'll look like one." If you give it not a number, you'll get something like this:

```text
...\Fluency\Console> dotnet run  ..\Examples\doubler.fl
Ready!

"fool's number"

Unhandled Exception: System.FormatException: Input string was not in a correct format.
   at System.Number.StringToNumber(ReadOnlySpan`1 str, NumberStyles options, NumberBuffer& number, NumberFormatInfo info, Boolean parseDecimal)
   (... more stuff , who cares)
```

So make sure you give your program the right input. In the future, I'll have an integer parsing function that doesn't blow up if you give it the wrong thing, but this is what we have for now.

The next useful thing is `Dup()`. `Dup()` takes something from the top pipeline and puts it on both the top and the bottom pipelines. Notice how we handle the bottom pipeline- there's a function on the next line with this `\.` sequence that says "connect before and above me", and so both `Com()` and `Com("n")` will get a copy of the value. Notice that lining your functions up is important! If you try to run this program, for instance:

```cs
Def(Main).ParseInt().Dup().Com().Add()
                \.Com("oops")./
```

You'll get an error message that looks like this:

```text
Unhandled Exception: Fluency.Execution.Exceptions.ExecutionException: Tried to use function Com as a IBottomIn, which it isn't. ---> System.InvalidCastException: Unable to cast object of type 'Fluency.Execution.Functions.BuiltIn.Com' to type 'Fluency.Execution.Functions.IBottomIn'.
(... other stuff)
```

Which is just telling you that "Com doesn't read input from its bottom pipeline, and so you shouldn't be connecting to it." If you want to know which pipelines a built in function reads from, click its name on the list of [builtin functions](~/api/Fluency.Execution.Functions.BuiltIn). For example, [Dup()](~/api/Fluency.Execution.Functions.BuiltIn.Dup) is an ITopIn, ITopOut, and IBottomOut, which means it reads from the top input and writes to both the top output and bottom output. Don't worry too much about the rest, though you will see that `Dup()` has an alias, `Duplicate()`, that does the same thing.

 You'll see `Dup()` a lot, because a common pattern in Fluency is:
- Copy the number
- Decide if you want it on the top
- Compute it on the bottom

We'll see later that this isn't wasteful and values are only computed when you ask for them!

You've seen `Com()` before as well, though we called it `I()` last time. `Com()` is short for `Comment()`, and it simply passes things from its input to its output without changing them. Comments are often used to denote what's in the pipeline at a given time (though you can also use regular old lines that start with `//`), and when you need to line things up. For example, here, `Com("n")` is simply passing `Dup()`'s lower output to `Add()`'s lower input, and the `Com()` on top is making sure the spacing works out. You might wonder what the `"n"` is doing in there. It's called an argument, and it's a way of making sure things get put on the function's pipeline. `Comment()` simply ignores its arguments so you can put whatever you want in there- but I suggest putting it in double quotes so the interpreter can still make sense of it!

The last new function is `Add()`. As you might guess, `Add()` reads something from the top and bottom pipelines, adds them together, and puts the results out on the top. `Add()` can also take an argument, like `Add(1)`. In this case, it reads a number from the top pipeline, adds one to it, and puts it on the top pipeline without touching the bottom at all. 

"Hey, wait." I hear you cry. "Can't I get rid of all those comments? It seems like they're just taking up space."
And you would be correct! A more succinct way to write this program would be:

```cs
Def(Main).Dup().Add()
            \.I()./
```

Notice that you still need something along the bottom to connect `Dup()` to `Add()`. Otherwise, with no argument, `Add()` will try to read from its lower pipeline and not be happy.

Can we do even better?

You probably know that when someone teaching you something asks that, they mean yes.

If you've seen the list of [builtin functions](~/api/Fluency.Execution.Functions.BuiltIn), you might know that there's also a `Mult()` function that does multiplication, and this program could also be written as:

```cs
Def(Main).ParseInt().Mult(2)
```

Which works just as well (and is even a touch faster), but I wrote this program before adding `.Mult()` to the language, and you had to learn about the bottom pipeline one way or another.

Next time, we'll talk about how to do things conditionally, and why the whole "things are only computed when you need them" trick is useful.
