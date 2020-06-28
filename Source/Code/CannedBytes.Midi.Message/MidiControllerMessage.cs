namespace CannedBytes.Midi.Message
{
    using System;

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
            if (Command != MidiChannelCommand.ControlChange)
            {
                throw new ArgumentException(
                    "Cannot construct a MidiControllerMessage instance other than MidiChannelCommand.Controller.", nameof(data));
            }
        }

        /// <summary>
        /// Gets the type of controller of the message.
        /// </summary>
        public MidiControllerType ControllerType
        {
            get
            {
                if (!IsKnownControllerType(Parameter1))
                {
                    return MidiControllerType.Unknown;
                }

                return (MidiControllerType)Parameter1;
            }
        }

        /// <summary>
        /// Gets the second parameter (usually the value of the controller) of the midi message.
        /// </summary>
        public byte Value
        {
            get
            {
                return Parameter2;
            }
        }

        /// <summary>
        /// Indicates if the specified <paramref name="controllerType"/> value is valid.
        /// </summary>
        /// <param name="controllerType">Usually taken from the first parameter of a midi controller message.</param>
        /// <returns>Returns true if it is a valid midi controller type value.</returns>
        public static bool IsKnownControllerType(byte controllerType)
        {
            return Enum.IsDefined(typeof(MidiControllerType), (int)controllerType);
        }
    }
}