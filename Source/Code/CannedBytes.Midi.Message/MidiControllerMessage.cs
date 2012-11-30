using System;

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
            #region Method Checks

            if (Command != MidiChannelCommand.ControlChange)
            {
                throw new ArgumentException(
                    "Cannot construct a MidiControllerMessage instance other than MidiChannelCommand.Controller.", "data");
            }

            #endregion Method Checks
        }

        /// <summary>
        /// Gets the type of controller of the message.
        /// </summary>
        public MidiControllerType ControllerType
        {
            get { return (MidiControllerType)base.Param1; }
        }

        /// <summary>
        /// Gets the second parameter (usually the value of the controller) of the midi message.
        /// </summary>
        public byte Param
        {
            get { return base.Param2; }
        }
    }
}