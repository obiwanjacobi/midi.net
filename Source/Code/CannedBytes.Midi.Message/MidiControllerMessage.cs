using System;
using System.Diagnostics.Contracts;

namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// Represents a midi continuous controller (CC) message.
    /// </summary>
    public class MidiControllerMessage : MidiChannelMessage
    {
        /// <summary>
        /// Constructs a new instance for the specified <paramref name="data"/>.
        /// </summary>
        /// <param name="data">Lower/least significant (max) 3 bytes are filled.</param>
        public MidiControllerMessage(int data)
            : base(data)
        {
            Contract.Ensures(Command == MidiChannelCommands.ControlChange);
            Contract.Ensures(Enum.IsDefined(typeof(MidiControllerTypes), (int)Param1));

            if (Command != MidiChannelCommands.ControlChange)
            {
                throw new ArgumentException(
                    "Cannot construct a MidiControllerMessage instance other than MidiChannelCommand.Controller.", "data");
            }

            if (!Enum.IsDefined(typeof(MidiControllerTypes), (int)Param1))
            {
                throw new ArgumentException(
                    "Invalid type of controller specified in data.", "data");
            }
        }

        /// <summary>
        /// Gets the type of controller of the message.
        /// </summary>
        public MidiControllerTypes ControllerType
        {
            get { return (MidiControllerTypes)base.Param1; }
        }

        /// <summary>
        /// Gets the second parameter (usually the value of the controller) of the midi message.
        /// </summary>
        public byte Value
        {
            get
            {
                Contract.Ensures(Contract.Result<byte>() == Param2);

                return base.Param2;
            }
        }
    }
}