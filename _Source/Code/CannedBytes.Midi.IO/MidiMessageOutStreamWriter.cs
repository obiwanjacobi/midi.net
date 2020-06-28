namespace CannedBytes.Midi.IO
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using CannedBytes.Midi.Message;

    /// <summary>
    /// Writes midi messages to an out-stream buffer.
    /// </summary>
    /// <remarks>Uses the <see cref="MidiStreamEventWriter"/> internally.</remarks>
    public class MidiMessageOutStreamWriter : DisposableBase
    {
        /// <summary>
        /// Constructs a new instance on the <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">Must not be null.</param>
        public MidiMessageOutStreamWriter(MidiBufferStream stream)
        {
            Contract.Requires(stream != null);
            Check.IfArgumentNull(stream, "stream");

            this.BaseStream = stream;
            this.StreamWriter = new MidiStreamEventWriter(stream);
        }

        /// <summary>
        /// Gets a reference to the buffer stream that is written to.
        /// </summary>
        protected MidiBufferStream BaseStream { get; private set; }

        /// <summary>
        /// Gets the stream event writer.
        /// </summary>
        protected MidiStreamEventWriter StreamWriter { get; private set; }

        /// <summary>
        /// Indicates if the stream has enough room to write the specified <paramref name="message"/> to the stream.
        /// </summary>
        /// <param name="message">Must no be null.</param>
        /// <returns>Returns true if the message can be written.</returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public virtual bool CanWrite(IMidiMessage message)
        {
            Contract.Requires(message != null);
            Check.IfArgumentNull(message, "message");

            var shortMessage = message as MidiShortMessage;

            if (shortMessage != null)
            {
                return this.StreamWriter.CanWriteShort();
            }

            var longMessage = message as MidiLongMessage;

            if (longMessage != null)
            {
                return this.StreamWriter.CanWriteLong(longMessage.GetData());
            }

            throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture, "The type '{0}' is not supported for message argument.", message.GetType().FullName));
        }

        /// <summary>
        /// Writes a new event to the stream for the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        /// <param name="deltaTime">The delta-time for the event.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public virtual void Write(IMidiMessage message, int deltaTime)
        {
            Contract.Requires(message != null);
            Check.IfArgumentNull(message, "message");

            var shortMessage = message as MidiShortMessage;

            if (shortMessage != null)
            {
                this.Write(shortMessage, deltaTime);
                return;
            }

            var longMessage = message as MidiLongMessage;

            if (longMessage != null)
            {
                this.Write(longMessage, deltaTime);
                return;
            }

            throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture, "The type '{0}' is not supported for message argument.", message.GetType().FullName));
        }

        /// <summary>
        /// Writes a new event to the stream for the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        /// <param name="deltaTime">The delta-time for the event.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public virtual void Write(MidiShortMessage message, int deltaTime)
        {
            Contract.Requires(message != null);
            Check.IfArgumentNull(message, "message");

            this.StreamWriter.WriteShort(message.Data, deltaTime);
        }

        /// <summary>
        /// Writes a new event to the stream for the <paramref name="message"/>.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        /// <param name="deltaTime">The delta-time for the event.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public virtual void Write(MidiLongMessage message, int deltaTime)
        {
            Contract.Requires(message != null);
            Check.IfArgumentNull(message, "message");

            var sysexMessage = message as MidiSysExMessage;

            if (sysexMessage != null)
            {
                this.StreamWriter.WriteLong(sysexMessage.GetData(), deltaTime);
            }

            throw new ArgumentException(
                String.Format(CultureInfo.InvariantCulture, "The type '{0}' is not supported for (long) message argument.", message.GetType().FullName));
        }

        /// <inheritdocs/>
        protected override void Dispose(DisposeObjectKind disposeKind)
        {
            if (!IsDisposed)
            {
                if (disposeKind == DisposeObjectKind.ManagedAndUnmanagedResources)
                {
                    this.StreamWriter.Dispose();
                }
            }
        }
    }
}