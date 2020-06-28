# `MIDI.NET`

`MIDI.NET` allows any .NET developer to access the power of MIDI without (the developer) having to do P/Invokes.

`MIDI.NET` provide .NET developers with an easy an robust way to employ MIDI in their applications.
It handles all unmanaged code for you and interfaces with the Windows Multimedia API. 
It consists of two major parts: the MidiPorts and the Midi Messages.

It is very much appriciated if you...[![Donate](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=HTE6LFLSC8RPL&lc=US&item_name=Canned%20Bytes&item_number=MIDI%2eNET&currency_code=EUR&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted)

**Or join the Discord Server** https://discord.gg/A8xe9YC

**Come visit `MIDI.NET` on Facebook.**
[https://www.facebook.com/midi4net](https://www.facebook.com/midi4net)

**NOTE**: For Windows 8 WinRT (StoreApps) you can use: [https://www.nuget.org/packages/Microsoft.WindowsPreview.MidiRT/](https://www.nuget.org/packages/Microsoft.WindowsPreview.MidiRT/)

----

## Midi Ports
The "Chain of Responsibilities" pattern is used to move MIDI data to and from the MidiPorts. ChainManager classes allow you to easily buildup these chains of processing and provide an excellent extension point for you to customize Midi data handling.
Buffers to handle Midi Streams (out) and System Exclusive Midi messages (in/out) are also handled by the library. These buffers are implemented as Streams and allow easy access of the Midi data contained within.

## Midi Messages
The wide range of Midi messages are broken down into a group of Midi Message classes that each represent a specific type of Midi message. Also the Meta messages found in Midi Files are present.
The "Factory" pattern is used to allow you to easily create these instances and even pool them for more efficient use of memory (this is also known as the "flyweight" pattern).

`MIDI.NET` runs on any Windows version except WinRT (it will run on the Windows 8 desktop) that has .NET 4.0 (or higher) .NET Core 3.1 installed. It is built using Visual Studio 2019.

## MIDI Resources
* MMA [http://www.midi.org/techspecs/index.php](http://www.midi.org/techspecs/index.php)
* MIDI Spec [http://web.archive.org/web/20070820161159/http://www.borg.com/~jglatt/tech/midispec.htm](http://web.archive.org/web/20070820161159/http://www.borg.com/~jglatt/tech/midispec.htm) 

**Canned Bytes**
The namespace of the MIDI.NET assemblies starts with CannedBytes. 
This is the name of the company I have started a couple of years ago. 
