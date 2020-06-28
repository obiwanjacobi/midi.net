namespace CannedBytes.Midi
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The MidiStreamException is throw when errors occur in a <see cref="T:MidiStream"/>.
    /// </summary>
    [Serializable]
    public class MidiStreamException : MidiException
    {
        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        public MidiStreamException()
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        public MidiStreamException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>
        /// and the <paramref name="inner"/>Exception.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        /// <param name="inner">The exception this instance will wrap.</param>
        public MidiStreamException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Must not be null.</param>
        /// <param name="context">Must not be null.</param>
        protected MidiStreamException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}