namespace CannedBytes.Midi.Message
{
    using System.Collections.Generic;

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
        private readonly Dictionary<int, MidiShortMessage> _msgPool = new Dictionary<int, MidiShortMessage>();

        /// <summary>
        /// Gets or sets an indication if to copy the long message data or reference it.
        /// </summary>
        public bool CopyData { get; set; }

        /// <summary>
        /// Creates a new short midi message object.
        /// </summary>
        /// <param name="message">A midi message of single byte.</param>
        /// <returns>Never returns null.</returns>
        public MidiShortMessage CreateShortMessage(byte message)
        {
            return CreateShortMessage((int)message);
        }

        /// <summary>
        /// Creates a new short midi message object.
        /// </summary>
        /// <param name="message">A full short midi message with the lower 3 bytes filled.</param>
        /// <returns>Never returns null.</returns>
        public MidiShortMessage CreateShortMessage(int message)
        {
            MidiShortMessage result = Lookup(message);

            if (result == null)
            {
                int statusChannel = MidiData.GetStatus(message);
                byte status = (byte)(statusChannel & (byte)0xF0);

                if (status == 0xF0)
                {
                    if (statusChannel >= 0xF8)
                    {
                        result = new MidiSysRealtimeMessage(message);
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

                Add(result);
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
            Check.IfArgumentOutOfRange<byte>(channel, 0, 15, nameof(channel));

            var data = new MidiData
            {
                Status = (byte)((int)command | channel),
                Parameter1 = parameter1,
                Parameter2 = parameter2
            };

            var message = (MidiChannelMessage)Lookup(data);

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

                Add(message);
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
            Check.IfArgumentOutOfRange<byte>(channel, 0, 15, nameof(channel));

            var data = new MidiData
            {
                Status = (byte)((int)MidiChannelCommand.ControlChange | channel),
                Parameter1 = (byte)controller,
                Parameter2 = value
            };

            var message = (MidiControllerMessage)Lookup(data);

            if (message == null)
            {
                message = new MidiControllerMessage(data);

                Add(message);
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
        public MidiSysExMessage CreateSysExMessage(byte[] longData, bool isContinuation)
        {
            Check.IfArgumentNull(longData, nameof(longData));

            return new MidiSysExMessage(CopyBuffer(longData), isContinuation);
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
        public MidiMetaMessage CreateMetaMessage(MidiMetaType metaType, byte[] longData)
        {
            Check.IfArgumentNull(longData, nameof(longData));

            var buffer = CopyBuffer(longData);

            if (MidiMetaTextMessage.IsMetaTextType(metaType))
            {
                return new MidiMetaTextMessage(metaType, buffer);
            }

            return new MidiMetaMessage(metaType, buffer);
        }

        /// <summary>
        /// Attempts to retrieve a short midi message from the pool.
        /// </summary>
        /// <param name="data">The short midi message data.</param>
        /// <returns>Returns null when no instance could be found.</returns>
        private MidiShortMessage Lookup(int data)
        {
            lock (_msgPool)
            {
                if (_msgPool.ContainsKey(data))
                {
                    return _msgPool[data];
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
            Check.IfArgumentNull(message, nameof(message));

            lock (_msgPool)
            {
                _msgPool.Add(message.Data, message);
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
            if (CopyData && data != null)
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