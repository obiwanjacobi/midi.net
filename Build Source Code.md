# Build the Source Code

The source code confirms to Code Analysis and StyleCop rules sets. It also uses Code Contracts.

**Note**: skip right to **Build the Samples** if you are only interested in working with the sample code.

## Prepare
* Make sure you have Visual Studio 2010 or higher. The free Express edition (C#) should work fine.
* Download the Source folder - the root folder for all sources. This will get you the sources to MIDI.NET and the Sample applications as well as the _SharedAssemblies - dependencies MIDI.NET relies on.
* Optional: [Install Code Contracts](http://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970). This will not work for the Visual Studio Express edition.

## Build MIDI.NET
**Note** you do not need to build the MIDI.NET code in order to work with it. Download the binaries from the Release page.

* Open the Source/Code/CannedBytes.MIDI.NET.sln solution file in Visual Studio.
* Visit each project and remove the AssemblyInfo.General.cs files from each projects Properties folder.
* Also make sure that Signing is off in the Project Property page (Alt-Enter).
* Rebuild the entire solution. You should have no errors and have output in the _SharedAssemblies folder.

## Build the Samples
You should have a full set of MIDI.NET assemblies in the _SharedAssemblies folder befor you start. Download them from the Release page or build them using the instructions above.

* Open the Source/Code/CannedBytes.MIDI.NET.Samples.sln solution file in Visual Studio.
* Rebuild the entire solution. You should have no errors.
