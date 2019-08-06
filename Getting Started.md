# Getting Started

The MIDI.NET API has been designed to be very easy to use. Here are some examples for common tasks when working with Midi.

## Send a Midi Message
How to send a short midi message to and output port.

The following code uses the first available midi out port to output a Reset message.

```
using CannedBytes.Midi;

var midiOut = new MidiOutPort();
midiOut.Open(0);

midiOut.ShortData(0xFF);
```

The {{ MidiOutPort.ShortData }} method takes an {{ int }}. It contains all the bytes that are used for a  short midi message (max 3 bytes).

To assist you in putting these separate bytes into one int the {{ MidiData }} struct can help out. The next example shows how to create a midi note-on message.

```
using CannedBytes.Midi;

var midiOut = new MidiOutPort();
midiOut.Open(0);

var midiData = new MidiData();
midiData.Status = 0x90 // note-on for channel 1
midiData.Paramater1 = 64; // note number
midiData.Parameter2 = 100; // velocity

midiOut.ShortData(midiData);
```
Because the {{ MidiData }} struct has an implicit {{ int }} operator you can pass it directly to the ShortData method.

## Receive a Midi Message
For receiving midi messages you need to provide 'something' to where these messages can be delivered. A object assigned to the {{ Successor }} property on the {{ MidiInPort }} will be called to receive the data.  You must implement the {{ IMidiDataReceiver }} interface for this. 
The  [Chain of Responsibilities pattern](http://en.wikipedia.org/wiki/Chain-of-responsibility_pattern) is used here to allow further processing of the received midi data.

```
using CannedBytes.Midi;

private class MyReceiver : IMidiDataReceiver
{
    public void ShortData(int data, long timestamp)
    {
        // your processing here. Keep it short!
    }

    public void LongData(MidiBufferStream buffer, long timestamp)
    {
        // not used for short midi messages
    }
}

var midiIn = new MidiInPort();
midiIn.Successor = new MyReceiver();
midiIn.Open(0);
midiIn.Start();

// .....

midiIn.Stop();
```
You must assign the {{ Successor }} property before calling the {{ Open }} method or you will get an exception. Likewise you cannot set the {{ Successor }} property once the port has been opened.
Do not forget to call {{ Start }} on the {{ MidiInPort }} for it to start receiving messages and deliver them to your implementation of {{ IMidiDataReceiver }}.

## Cleaning up
A call to {{ Close }} on the midi port will cleanup all used (unmanaged) resources. 
When a midi port remains open because you forgot to call {{ Close }} on it, other programs on your system (including your own) cannot open that Midi Port - unless your Midi driver supports multiple clients. So it is always a good practice to close all ports when your done with them.

## Discover Midi Ports
There is an easy way to discover what midi ports are available on the machine. A read-only collection can be instantiated that will contain the midi port capabilities for all midi ports on the machine.

```
using CannedBytes.Midi;

var midiInCaps = new MidiInPortCapsCollection();

foreach(var inCaps in midiInCaps)
{
    Console.Write(inCaps.Name);
}
```
The collection items are instances of the {{ MidiInPortCaps }} class. To discover midi out port capabilities, instantiate the {{ MidiOutPortCapsCollection }} class that will give you a collection of {{ MidiOutPortCaps }} items. 

In order to open a specific midi port, the collection index of that port in the Caps collection can be used as a portId.
```
using CannedBytes.Midi;

var midiInCaps = new MidiInPortCapsCollection();
var selectedCaps = midiInCaps[??](__);  // selected midi port

int portId = midiInCaps.IndexOf(selectedCaps);

var midiIn = new MidiInPort();
midiIn.Successor = new MyReceiver();
midiIn.Open(portId);

....
```
Again, the same method can be used for opening {{ MidiOutPort }}s.
