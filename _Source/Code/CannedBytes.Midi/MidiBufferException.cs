namespace CannedBytes.Midi
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when there is a problem with a <see cref="MidiBufferStream"/>.
    /// </summary>
    [Serializable]
    public class MidiBufferException : MidiException
    {
        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        public MidiBufferException()
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        public MidiBufferException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>
        /// and the <paramref name="inner"/>Exception.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        /// <param name="inner">The exception this instance will wrap.</param>
        public MidiBufferException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Serialization constructor.
        /// </summary>
        /// <param name="info">Must not be null.</param>
        /// <param name="context">Must not be null.</param>
        protected MidiBufferException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}