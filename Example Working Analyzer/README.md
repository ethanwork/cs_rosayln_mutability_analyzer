Open up the analyzer1 solution, and then select the vsix project as the startup project. Then run it, and it will then start up a new visual studio 2022 instance for you, then select the console app 3 project, it should after a bit highlight the ImmutableArray<int>.Empty.Add(1) with a squiggly saying this is not a good way to create an immutable array based off our  analyzer.

So basically, create your analyzer code, then set the vsix project as the startup, then open up a sample project.

This was based off this youtube video.
https://www.youtube.com/watch?v=Hu_rtcNoVgc