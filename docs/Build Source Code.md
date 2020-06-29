# Build the Source Code

**Note**: skip right to **Build the Samples** if you are only interested in working with the sample code.

## Prepare
* Make sure you have Visual Studio 2019 or higher.
* Download the Source folder - the root folder for all sources. This will get you the sources to `MIDI.NET` and the Sample applications.
All NuGet packages should resolve automatically.

## Build MIDI.NET
**Note** you do not need to build the `MIDI.NET` code in order to work with it.
Include the `CannedBytes.Midi` NuGet packages in your project to use MIDI.NET.

* Open the Source/Code/CannedBytes.Midi.sln solution file in Visual Studio 2019.
* Rebuild the entire solution. You should have no errors and have output in the _SharedAssemblies folder.

## Build the Samples

* Open the Source/Samples/CannedBytes.Midi.Samples.sln solution file in Visual Studio 2019.
The dependent `CannedBytes.Midi` NuGet packages should resolve automatically.
* Rebuild the entire solution. You should have no errors.
