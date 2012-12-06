using System;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutPortException is thrown when errors occur with the
    /// <see cref="T:MidiOutPort"/>.
    /// </summary>
    [Serializable]
    public class MidiOutPortException : MidiPortException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        /// <summary>
        /// Constructs an empty instance.
        /// </summary>
        public MidiOutPortException() { }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        public MidiOutPortException(string message) : base(message) { }

        /// <summary>
        /// Constructs an instance containing the specified <paramref name="message"/>
        /// and the <paramref name="inner"/>Exception.
        /// </summary>
        /// <param name="message">The exception message text.</param>
        /// <param name="inner">The exception this instance will wrap.</param>
        public MidiOutPortException(string message, Exception inner) : base(message, inner) { }

        /// <summary>
        /// Serialization constructor
        /// </summary>
        /// <param name="info">Must not be null.</param>
        /// <param name="context">Must not be null.</param>
        protected MidiOutPortException(
          global::System.Runtime.Serialization.SerializationInfo info,
          global::System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}