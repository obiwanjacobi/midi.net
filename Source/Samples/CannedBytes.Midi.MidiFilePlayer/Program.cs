using System;
using System.Collections.Generic;
using System.Linq;
using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;
using System.Threading;

namespace CannedBytes.Midi.MidiFilePlayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string midiFileName = null;
            int outPortId = 0;

            if (args.Length > 0)
            {
                midiFileName = args[0];
            }

            if (args.Length > 1)
            {
                outPortId = Int16.Parse(args[1]);
            }

            if (args.Length <= 0)
            {
                Console.WriteLine("Usage: MidiFilePlayer 'C:\\Folder\\MidiFile.mid' [MidiOutPortId]");
                return;
            }

            Console.WriteLine("Reading midi file: " + midiFileName);
            var fileData = ReadMidiFile(midiFileName);

            IEnumerable<MidiFileEvent> notes = null;

            // merge all track notes and filter out sysex and meta events
            foreach (var track in fileData.Tracks)
            {
                if (notes == null)
                {
                    notes = from note in track.Events
                            where !(note.Message is MidiLongMessage)
                            select note;
                }
                else
                {
                    notes = (from note in track.Events
                             where !(note.Message is MidiLongMessage)
                             select note).Union(notes);
                }
            }

            // order track notes by absolute-time.
            notes = from note in notes
                    orderby note.AbsoluteTime
                    select note;

            // At this point the DeltaTime properties are invalid because other events from other 
            // tracks are now merged between notes where the initial delta-time was calculated for.
            // We fix this in the play back routine.

            WriteHeaderInfoToConsole(fileData.Header);

            var caps = MidiOutPort.GetPortCapabilities(outPortId);
            MidiOutPortBase outPort = null;

            try
            {
                outPort = ProcessStreaming(outPortId, fileData, notes, caps);
            }
            catch (Exception e)
            {
                Console.WriteLine();

                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();

            if (outPort != null)
            {
                outPort.Reset();
                if (!outPort.MidiBufferManager.WaitForBuffersReturned(1000))
                {
                    Console.WriteLine("Buffers failed to return in 1 sec.");
                }

                outPort.Close();
                outPort.Dispose();
            }
        }

        private static MidiOutPortBase ProcessStreaming(int outPortId, MidiFileData fileData,
            IEnumerable<MidiFileEvent> notes, MidiOutPortCaps caps)
        {
            var outPort = new MidiOutStreamPort();
            outPort.Open(outPortId);
            outPort.MidiBufferManager.Initialize(10, 1024);
            outPort.TimeDivision = fileData.Header.TimeDivision;

            // TODO: extract Tempo from meta messages from the file.
            // 120 bpm (uSec/QuarterNote).
            outPort.Tempo = 500000;

            Console.WriteLine(String.Format("Midi Out Stream Port '{0}' is now open.", caps.Name));

            MidiMessageOutStreamWriter writer = null;
            MidiBufferStream buffer = null;
            MidiFileEvent lastNote = null;

            foreach (var note in notes)
            {
                if (writer == null)
                {
                    // brute force buffer aqcuirement.
                    // when callbacks are implemented this will be more elegant.
                    do
                    {
                        buffer = outPort.MidiBufferManager.Retrieve();

                        if (buffer != null) break;

                        Thread.Sleep(50);

                    } while (buffer == null);

                    writer = new MidiMessageOutStreamWriter(buffer);
                }

                if (writer.CanWrite(note.Message))
                {
                    if (lastNote != null)
                    {
                        // fixup delta time artifically...
                        writer.Write(note.Message, (int)(note.AbsoluteTime - lastNote.AbsoluteTime));
                    }
                    else
                    {
                        writer.Write(note.Message, (int)note.DeltaTime);
                    }
                }
                else
                {
                    outPort.LongData(buffer);
                    writer = null;

                    Console.WriteLine("Buffer sent...");

                    if (!outPort.HasStatus(MidiPortStatus.Started))
                    {
                        outPort.Restart();
                    }
                }

                lastNote = note;
            }

            return outPort;
        }

        private static void WriteHeaderInfoToConsole(MThdChunk mThdChunk)
        {
            Console.WriteLine("Number of tracks: " + mThdChunk.NumberOfTracks);
            Console.WriteLine("Number of format: " + mThdChunk.Format);
            Console.WriteLine("Number of time division: " + mThdChunk.TimeDivision);
        }

        internal static MidiFileData ReadMidiFile(string filePath)
        {
            var data = new MidiFileData();
            var reader = FileReaderFactory.CreateReader(filePath);

            data.Header = reader.ReadNextChunk() as MThdChunk;

            var tracks = new List<MTrkChunk>();

            for (int i = 0; i < data.Header.NumberOfTracks; i++)
            {
                var track = reader.ReadNextChunk() as MTrkChunk;

                if (track != null)
                {
                    tracks.Add(track);
                }
                else
                {
                    Console.WriteLine(String.Format("Track '{0}' was not read successfully.", i + 1));
                }
            }

            data.Tracks = tracks;
            return data;
        }
    }
}