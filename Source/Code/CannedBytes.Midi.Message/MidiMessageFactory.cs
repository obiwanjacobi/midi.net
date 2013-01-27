namespace CannedBytes.Midi.Message
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// A factory class for creating midi message objects.
    /// </summary>
    /// <remarks>Short midi messages are pooled.
    /// This means that no more than once instance will ever be created
    /// (by this factory) for the exact same midi message.</remarks>
    public class MidiMessageFactory
    {
        /// <summary>
        /// Maintains the pooled short midi messages.
        /// </summary>
        private Dictionary<int, MidiShortMessage> msgPool = new Dictionary<int, MidiShortMessage>();

        /// <summary>
        /// The object's invariant contract.
        /// </summary>
        [ContractInvariantMethod]
        private void InvariantContract()
        {
            Contract.Invariant(this.msgPool != null);
        }

        /// <summary>
        /// Creates a new short midi message object.
        /// </summary>
        /// <param name="message">A midi message of single byte.</param>
        /// <returns>Never returns null.</returns>
        public MidiShortMessage CreateShortMessage(byte message)
        {
            Contract.Ensures(Contract.Result<MidiShortMessage>() != null);

            return this.CreateShortMessage((int)message);
        }

        /// <summary>
        /// Gets or sets an indication if to copy the long message data or reference it.
        /// </summary>
        public bool CopyData { get; set; }

        /// <summary>
        /// Creates a new short midi message object.
        /// </summary>
        /// <param name="message">A full short midi message with the lower 3 bytes filled.</param>
        /// <returns>Never returns null.</returns>
        public MidiShortMessage CreateShortMessage(int message)
        {
            Contract.Ensures(Contract.Result<MidiShortMessage>() != null);

            MidiShortMessage result = this.Lookup(message);

            if (result == null)
            {
                int statusChannel = MidiData.GetStatus(message);
                byte status = (byte)(statusChannel & (byte)0xF0);

                if (status == 0xF0)
                {
                    if (statusChannel >= 0xF8)
                    {
                        result = new MidiSysRealTimeMessage(message);
                    }
                    else
                    {
                        result = new MidiSysCommonMessage(message);
                    }
                }
                else if (status == (byte)MidiChannelCommand.ControlChange)
                {
                    result = new MidiControllerMessage(message);
                }
                else
                {
                    result = new MidiChannelMessage(message);
                }

                this.Add(result);
            }

            return result;
        }

        /// <summary>
        /// Creates a new channel (short) midi message object.
        /// </summary>
        /// <param name="command">The channel command.</param>
        /// <param name="channel">The (zero-based) channel number.</param>
        /// <param name="parameter1">The (optional) first parameter of the midi message.</param>
        /// <param name="parameter2">The (optional) second parameter of the midi message.</param>
        /// <returns>Never returns null.</returns>
        public MidiChannelMessage CreateChannelMessage(MidiChannelCommand command, byte channel, byte parameter1, byte parameter2)
        {
            Contract.Requires(channel >= 0 && channel <= 15);
            Contract.Ensures(Contract.Result<MidiChannelMessage>() != null);
            Check.IfArgumentOutOfRange<byte>(channel, 0, 15, "channel");

            MidiData data = new MidiData();
            data.Status = (byte)((int)command | channel);
            data.Parameter1 = parameter1;
            data.Parameter2 = parameter2;

            MidiChannelMessage message = (MidiChannelMessage)this.Lookup(data);

            if (message == null)
            {
                if (command == MidiChannelCommand.ControlChange)
                {
                    message = new MidiControllerMessage(data);
                }
                else
                {
                    message = new MidiChannelMessage(data);
                }

                this.Add(message);
            }

            return message;
        }

        /// <summary>
        /// Creates a new midi controller message object.
        /// </summary>
        /// <param name="channel">The (zero-based) midi channel number.</param>
        /// <param name="controller">The type of continuous controller.</param>
        /// <param name="value">The (optional) parameter (usually value) of the controller.</param>
        /// <returns>Returns a new instance.</returns>
        public MidiControllerMessage CreateControllerMessage(byte channel, MidiControllerType controller, byte value)
        {
            Contract.Ensures(Contract.Result<MidiControllerMessage>() != null);
            Check.IfArgumentOutOfRange<byte>(channel, 0, 15, "channel");

            MidiData data = new MidiData();
            data.Status = (byte)((int)MidiChannelCommand.ControlChange | channel);
            data.Parameter1 = (byte)controller;
            data.Parameter2 = value;

            MidiControllerMessage message = (MidiControllerMessage)this.Lookup(data);

            if (message == null)
            {
                message = new MidiControllerMessage(data);

                this.Add(message);
            }

            return message;
        }

        /// <summary>
        /// Creates a new System Exclusive midi message object.
        /// </summary>
        /// <param name="longData">The full data for the sysex (including the begin and end markers). Must not be null or empty.</param>
        /// <param name="isContinuation">An indication if this message is a continuation of a previous sysex message.</param>
        /// <returns>Never returns null.</returns>
        /// <remarks>The SysEx message objects are NOT pooled.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Check is not recognized.")]
        public MidiSysExMessage CreateSysExMessage(byte[] longData, bool isContinuation)
        {
            Contract.Requires(longData != null);
            Contract.Requires(longData.Length > 0);
            Contract.Ensures(Contract.Result<MidiSysExMessage>() != null);
            Check.IfArgumentNull(longData, "longData");

            return new MidiSysExMessage(this.CopyBuffer(longData), isContinuation);
        }

        /// <summary>
        /// Creates a new Meta midi message object.
        /// </summary>
        /// <param name="metaType">The type of meta message.</param>
        /// <param name="longData">The data of the meta message. Must not be null or empty.</param>
        /// <returns>Never returns null.</returns>
        /// <remarks>The Meta message objects are NOT pooled.
        /// For some <paramref name="metaType"/> value a <see cref="MidiMetaTextMessage"/>
        /// instance is returned.</remarks>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Check is not recognized.")]
        public MidiMetaMessage CreateMetaMessage(MidiMetaType metaType, byte[] longData)
        {
            Contract.Requires(metaType != MidiMetaType.Unknown);
            Contract.Requires(longData != null);
            Contract.Ensures(Contract.Result<MidiMetaMessage>() != null);
            Check.IfArgumentNull(longData, "longData");

            var buffer = this.CopyBuffer(longData);

            switch (metaType)
            {
                case MidiMetaType.Copyright:
                case MidiMetaType.CuePoint:
                case MidiMetaType.Custom:
                case MidiMetaType.DeviceName:
                case MidiMetaType.Instrument:
                case MidiMetaType.Lyric:
                case MidiMetaType.Marker:
                case MidiMetaType.PatchName:
                case MidiMetaType.Text:
                case MidiMetaType.TrackName:
                    return new MidiMetaTextMessage(metaType, buffer);
                default:
                    return new MidiMetaMessage(metaType, buffer);
            }
        }

        /// <summary>
        /// Attempts to retrieve a short midi message from the pool.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <returns>Returns null when no instance could be found.</returns>
        private MidiShortMessage Lookup(int data)
        {
            lock (this.msgPool)
            {
                if (this.msgPool.ContainsKey(data))
                {
                    return this.msgPool[data];
                }
            }

            return null;
        }

        /// <summary>
        /// Add the <paramref name="message"/> to the pool.
        /// </summary>
        /// <param name="message">Must not be null.</param>
        private void Add(MidiShortMessage message)
        {
            Contract.Requires(message != null);
            Check.IfArgumentNull(message, "message");

            lock (this.msgPool)
            {
                this.msgPool.Add(message.Data, message);
            }
        }

        /// <summary>
        /// Handles copying a <paramref name="data"/> buffer.
        /// </summary>
        /// <param name="data">Can be null.</param>
        /// <returns>Returns the new buffer with the same contents.
        /// Returns null if <paramref name="data"/> was null.</returns>
        private byte[] CopyBuffer(byte[] data)
        {
            if (this.CopyData && data != null)
            {
                var buffer = new byte[data.Length];

                for (int i = 0; i < data.Length; i++)
                {
                    buffer[i] = data[i];
                }

                return buffer;
            }

            return data;
        }
    }
}