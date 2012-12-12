namespace CannedBytes.Midi.Message
{
    using System;
    using System.Diagnostics.Contracts;

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
            Contract.Ensures(Command == MidiChannelCommand.ControlChange);
            Contract.Ensures(Enum.IsDefined(typeof(MidiControllerType), (int)Parameter1));

            if (Command != MidiChannelCommand.ControlChange)
            {
                throw new ArgumentException(
                    "Cannot construct a MidiControllerMessage instance other than MidiChannelCommand.Controller.", "data");
            }

            if (!Enum.IsDefined(typeof(MidiControllerType), (int)Parameter1))
            {
                throw new ArgumentException(
                    "Invalid type of controller specified in data.", "data");
            }
        }

        /// <summary>
        /// Gets the type of controller of the message.
        /// </summary>
        public MidiControllerType ControllerType
        {
            get { return (MidiControllerType)Parameter1; }
        }

        /// <summary>
        /// Gets the second parameter (usually the value of the controller) of the midi message.
        /// </summary>
        public byte Value
        {
            get
            {
                Contract.Ensures(Contract.Result<byte>() == Parameter2);

                return Parameter2;
            }
        }
    }
}