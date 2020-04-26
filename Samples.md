# Samples

The following sample applications are available for MIDI.NET.

## CannedBytes.Midi.Mapper
This is a stand-alone WinForms application that implements a Midi mapper. Incoming Midi events can be transformed or mapped to (other) outgoing Midi events.

**Demonstrates** basic short Midi event handling.

| **File** | **Description** |
| Programs.cs |  Entry point for the application. |
| MidiMapperDocument.cs | Contains all settings for the note mapper. |
| MidiNoteMapper | Manages the Midi ports and processing. |
| MidiNoteMap | A collection of Map items that forms the Midi map. |
| MidiNoteMapItem | Definition for one incoming Midi event to one out going Midi event. |
| MidiNoteMapIndex | A fast lookup table for runtime finding of Map items. |
| UI/MainForm.cs | Main window form. |
| UI/MapItemForm.cs | UI for editing one Map Item. |
| UI/NoteMapView.cs | UI for the complete Midi Map. |
| UI/NoteNumberControl.cs | A edit control that calculates note numbers and names. |

## CannedBytes.Midi.MidiFilePlayer
This console program plays the midi file passed on the command line. The Midi File is opened and read into memory. That memory representation is then written into MidiBufferStreams and output to a MidiOutStreamPort.

**Demonstrates** reading Midi Files and using the MidiOutStreamPort including the MidiMessageOutStreamWriter to fill the MidiBufferStream instances.

| **File** | **Description** |
| Programs.cs |  Entry point for the application and main code. |
| MidiFileData.cs | Structure that contains all data from a Midi File. |
| FileReaderFactory.cs | Internal factory methods for creating the correct context for reading the Midi File.  |

## CannedBytes.Midi.RecordToFile
This WPF application allows you to record Midi events into memory. When the recording stops the user is prompted to save the Midi to file.

**Demonstrates** writing Midi Files. 

| **File** | **Description** |
| App.xaml |  Entry point for the application. |
| AppData.cs | Container of all application data. |
| Midi/MidiFileSerializer.cs | Implements writing a Midi File. |
| Midi/MidiReceiver.cs | Represents the object that receives the incoming Midi events. |
| Midi/MidiTrackBuilder.cs | Performs (post) processing to the recorded Midi tracks for saving to file. |
| UI/AppCommands | Central declaration of application commands. |
| UI/CommandHandler.cs | A base class for handling commands. |
| UI/MainWindow.xaml | The main window of the application containing all UI. |
| UI/StartStopCommandHandler.cs | Handles starting/stopping of the Midi recording. |
| UI/UpdatingRoutedUICommand.cs | A RoutedUICommand variation that notifies of changing its property values. |

## CannedBytes.Midi.SysExUtil
This WPF application allows you to record and send System Exclusive Midi messages (sysex). Sysex can also be persisted to the file system.

**Demonstrates** working with long midi messages and MidiBufferStream.

| **File** | **Description** |
| App.xaml |  Entry point for the application. |
| AppData.cs | Container of all application data. |
| Midi/MidiSysExBuffer.cs | A buffer for a sysex message. |
| Midi/MidiSysExReceiver.cs | Manages receiving Midi sysex messages. |
| Midi/MidiSysExSender.cs | Manages sending Midi sysex messages. |
| Persistence/SysExSerializer.cs | Manages loading/saving .syx files |
| UI/AppCommands | Central declaration of application commands. |
| UI/CommandHandler.cs | A base class for handling commands. |
| UI/FileNewCommandHandler.cs | Implements the File New command. |
| UI/FileOpenCommandHandler.cs | Implements the File Open command. |
| UI/FileSaveCommandHandler.cs | Implements the File Save command. |
| UI/MainWindow.xaml | The main window of the application containing all UI. |
| UI/PlayCommandHandler.cs | Implements sending the sysex to an out port. |
| UI/StartStopCommandHandler.cs | Handles starting/stopping of the Midi recording. |
| UI/UpdatingRoutedUICommand.cs | A RoutedUICommand variation that notifies of changing its property values. |

