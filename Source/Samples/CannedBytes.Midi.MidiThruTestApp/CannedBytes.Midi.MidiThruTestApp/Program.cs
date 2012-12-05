using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace CannedBytes.Midi.MidiThruTestApp
{
    class Program
    {
        static IntPtr inPort;
        static IntPtr outPort;

        static void Main(string[] args)
        {
            uint inPortId = 0;
            uint outPortId = 0;

            try
            {
                if (args.Length == 2)
                {
                    inPortId = uint.Parse(args[0]);
                    outPortId = uint.Parse(args[1]);
                }
                else if (args.Length != 0)
                {
                    PrintUsage();
                    return;
                }

                int result = NativeMethods.midiOutOpen(out outPort, outPortId, null, IntPtr.Zero, 0);

                if (result != NativeMethods.MMSYSERR_NOERROR)
                {
                    StringBuilder text = new StringBuilder(NativeMethods.MAXERRORLENGTH);
                    NativeMethods.midiOutGetErrorText(result, text, text.Capacity);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(text.ToString());
                    return;
                }

                var outCaps = new MidiOutCaps();
                NativeMethods.midiOutGetDevCaps(outPort, ref outCaps, (uint)Marshal.SizeOf(outCaps));

                Console.WriteLine("Midi Out Port is open: " + outCaps.name);

                result = NativeMethods.midiInOpen(out inPort, inPortId, MidiInProc, IntPtr.Zero, NativeMethods.CALLBACK_FUNCTION);

                if (result != NativeMethods.MMSYSERR_NOERROR)
                {
                    NativeMethods.midiOutClose(outPort);

                    StringBuilder text = new StringBuilder(NativeMethods.MAXERRORLENGTH);
                    NativeMethods.midiInGetErrorText(result, text, text.Capacity);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(text.ToString());
                    return;
                }

                var inCaps = new MidiInCaps();
                NativeMethods.midiInGetDevCaps(inPort, ref inCaps, (uint)Marshal.SizeOf(inCaps));

                Console.WriteLine("Midi In Port is open: " + inCaps.name);

                // start receiving midi
                NativeMethods.midiInStart(inPort);

                Console.WriteLine("Press any key to exit");

                while (!Console.KeyAvailable)
                {
                    Thread.Sleep(1000);
                }
            }
            catch(Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("CannedBytes.MidiMidiThruTestApp InPortId OutPortId");
        }

        private static void MidiOutProc(IntPtr handle, uint msg, IntPtr instance, IntPtr param1, IntPtr param2)
        {
            // no op, just marshalling the callback.
        }

        private static void MidiInProc(IntPtr handle, uint msg, IntPtr instance, IntPtr param1, IntPtr param2)
        {
            //try
            //{
                if (msg == NativeMethods.MIM_DATA)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    NativeMethods.midiOutShortMsg(outPort, (uint)param1.ToInt32());

                    sw.Stop();
                    Console.WriteLine("Midi Out ms:" + ((float)sw.ElapsedTicks / (float)Stopwatch.Frequency) * 1000);
                }
            //}
            //catch (Exception e)
            //{
            //    // Do not leak any exceptions into the calling windows code.

            //    // TODO: log error
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine(e);

            //    Console.ForegroundColor = ConsoleColor.Gray;

            //}
        }
    }
}
