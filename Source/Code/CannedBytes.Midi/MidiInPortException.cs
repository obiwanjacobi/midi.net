namespace CannedBytes.Midi
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The MidiOutPortException is thrown when errors occur with the
    /// <see cref="T:MidiInPort"/>.
    /// </summary>
    [Serializable]
    public class MidiInPortException : MidiPortException
    {
        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        public MidiInPortException()
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        public MidiInPortException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>
        /// and the <paramref name="inner"/>Exception.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        /// <param name="inner">The exception this instance will wrap.</param>
        public MidiInPortException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Must not be null.</param>
        /// <param name="context">Must not be null.</param>
        protected MidiInPortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}