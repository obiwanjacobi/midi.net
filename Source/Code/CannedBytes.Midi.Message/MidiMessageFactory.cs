using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace CannedBytes.Midi.Message
{
    /// <summary>
    /// A factory class for creating midi message objects.
    /// </summary>
    /// <remarks>Short midi messages are pooled.
    /// This means that no more than once instance will ever be created
    /// (by this factory) for the exact same midi message.</remarks>
    public class MidiMessageFactory
    {
        private Dictionary<int, MidiShortMessage> msgPool = new Dictionary<int, MidiShortMessage>();

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

            return CreateShortMessage((int)message);
        }

        /// <summary>
        /// Creates a new short midi message object.
        /// </summary>
        /// <param name="message">A full short midi message with the lower 3 bytes filled.</param>
        /// <returns>Never returns null.</returns>
        public MidiShortMessage CreateShortMessage(int message)
        {
            Contract.Ensures(Contract.Result<MidiShortMessage>() != null);

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
                else if (status == (byte)MidiChannelCommands.ControlChange)
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
        /// <param name="param1">The (optional) first parameter of the midi message.</param>
        /// <param name="param2">The (optional) second parameter of the midi message.</param>
        /// <returns>Never returns null.</returns>
        public MidiChannelMessage CreateChannelMessage(MidiChannelCommands command,
            byte channel, byte param1, byte param2)
        {
            Contract.Ensures(Contract.Result<MidiChannelMessage>() != null);
            Throw.IfArgumentOutOfRange<byte>(channel, 0, 15, "channel");

            MidiData data = new MidiData();
            data.Status = (byte)((int)command | channel);
            data.Parameter1 = param1;
            data.Parameter2 = param2;

            MidiChannelMessage message = (MidiChannelMessage)Lookup(data);

            if (message == null)
            {
                if (command == MidiChannelCommands.ControlChange)
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
        /// <param name="param">The (optional) parameter (usually value) of the controller.</param>
        /// <returns></returns>
        public MidiControllerMessage CreateControllerMessage(byte channel,
            MidiControllerTypes controller, byte param)
        {
            Contract.Ensures(Contract.Result<MidiControllerMessage>() != null);
            Throw.IfArgumentOutOfRange<byte>(channel, 0, 15, "channel");

            MidiData data = new MidiData();
            data.Status = (byte)((int)MidiChannelCommands.ControlChange | channel);
            data.Parameter1 = (byte)controller;
            data.Parameter2 = param;

            MidiControllerMessage message = (MidiControllerMessage)Lookup(data);

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
        /// <returns>Never returns null.</returns>
        /// <remarks>The SysEx message objects are NOT pooled.</remarks>
        public MidiSysExMessage CreateSysExMessage(byte[] longData)
        {
            Contract.Requires(longData != null);
            Contract.Requires(longData.Length > 0);
            Contract.Ensures(Contract.Result<MidiSysExMessage>() != null);
            Throw.IfArgumentNull(longData, "longData");

            return new MidiSysExMessage(longData);
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
        public MidiMetaMessage CreateMetaMessage(MidiMetaTypes metaType, byte[] longData)
        {
            Contract.Requires(metaType != MidiMetaTypes.Unknown);
            Contract.Requires(longData != null);
            Contract.Requires(longData.Length > 0);
            Contract.Ensures(Contract.Result<MidiMetaMessage>() != null);
            Throw.IfArgumentNull(longData, "longData");

            switch (metaType)
            {
                case MidiMetaTypes.Copyright:
                case MidiMetaTypes.CuePoint:
                case MidiMetaTypes.Custom:
                case MidiMetaTypes.DeviceName:
                case MidiMetaTypes.Instrument:
                case MidiMetaTypes.Lyric:
                case MidiMetaTypes.Marker:
                case MidiMetaTypes.PatchName:
                case MidiMetaTypes.Text:
                case MidiMetaTypes.TrackName:
                    return new MidiMetaTextMessage(metaType, longData);
                default:
                    return new MidiMetaMessage(metaType, longData);
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
            Throw.IfArgumentNull(message, "message");

            lock (this.msgPool)
            {
                this.msgPool.Add(message.Data, message);
            }
        }
    }
}