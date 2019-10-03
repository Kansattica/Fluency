# Welcome! To your first program.

You can check out the examples in the [examples folder](https://github.com/Kansattica/Fluency/tree/master/Examples), and I recommend it, but you can do that later. Right now, let's start with the basics. Create yourself a file. Call it anything you like, as long as it ends in `.fl`. I'm going to call mine `coolfile.fl`. It'll be easiest if you put it in the Console folder. Open it with your text editor of choice and put this in it. It's the shortest legal Fluency program. You might have to hit enter to make sure there's a blank line after it.

```cs
Def(Main).I()
```

That's it! Go ahead and `dotnet run coolfile.fl` (or whatever you called it) from the Console folder if you're compiling yourself, or `./fluency coolfile.fl` if you downloaded a release. Your computer will make you wait a while while it compiles Fluency and processes the source code into something it can use, but soon, you'll see something like this:

```text
C:\Users\Grace\Documents\Sauce\Fluency\Console> dotnet run .\coolfile.fl
Ready!

```

That Ready! is Fluency telling you it's ready. Fluency tries to be very straightforward when it can. Feel free to type something in here, hit Enter, and watch Fluency dutifully echo it back at you. When you're done playing, simply hit the enter key twice to add two blank lines or type ctrl-C. Fluency will say `Done` and exit. Here's the session I just did:

```text
C:\Users\Grace\Documents\Sauce\Fluency\Console> dotnet run .\coolfile.fl
Ready!

Hello!
Hello!
Holla
Holla
Get dolla
Get dolla
1000
1000
true false
true false
*/-*/*-/-*/-/-/-/-/-*/
*/-*/*-/-*/-/-/-/-/-*/


Done
```

And that's it! You just wrote your first Fluency program. On the next page, I'll dissect this program and tell you what it does and how to make it do something more interesting than provide sparkling conversation.