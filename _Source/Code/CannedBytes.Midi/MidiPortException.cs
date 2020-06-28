namespace CannedBytes.Midi
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The MidiPortException serves as a base class for specialized
    /// port exception types.
    /// </summary>
    /// <remarks>
    /// <seealso cref="MidiInPortException"/>
    /// <seealso cref="MidiOutPortException"/>
    /// </remarks>
    [Serializable]
    public class MidiPortException : MidiException
    {
        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        public MidiPortException()
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        public MidiPortException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>
        /// and the <paramref name="inner"/>Exception.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        /// <param name="inner">The exception this instance will wrap.</param>
        public MidiPortException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Must not be null.</param>
        /// <param name="context">Must not be null.</param>
        protected MidiPortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}