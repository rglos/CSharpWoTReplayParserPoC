C# World of Tanks Replay Parser Proof of Concept
========================

Proof of concept using C# to parse the World of Tanks replay files.

We have a working example of using python to parse the replay files - https://github.com/marklr/wotanalysis/tree/master/poc.

Can we integrate this into C# so that we can parse the file straight into an object that C# can consume?

~~Early research I did showed me this wasn't possible - http://stackoverflow.com/questions/14922722/how-i-can-deserialize-python-pickles-in-c~~

Proven wrong.

To get this working you'll need: (TODO: Should we just include the packages directory as part of the source... it would eliminate the NuGet step entirely i.e. get source, F5/F6, done.)

NuGet - Newtonsoft.Json

NuGet - IronPython.StdLib
