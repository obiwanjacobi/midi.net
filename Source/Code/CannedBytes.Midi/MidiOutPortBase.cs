using System;
using System.Diagnostics.Contracts;
using System.Text;

namespace CannedBytes.Midi
{
    /// <summary>
    /// The MidiOutPortBase class represent the common implementation for
    /// both the <see cref="MidiOutPort"/> and the <see cref="MidiStreamOutPort"/>.
    /// </summary>
    public abstract class MidiOutPortBase : MidiPort, IMidiSender
    {
        /// <summary>
        /// Turns off all notes and returns pending <see cref="MidiBufferStream"/>s to the <see cref="P:BufferManager"/>
        /// marked as done.
        /// </summary>
        public override void Reset()
        {
            int result = NativeMethods.midiOutReset(MidiSafeHandle);

            ThrowIfError(result);

            base.Reset();
        }

        /// <summary>
        /// Retrieves the Midi Stream Out Port capabilities.
        /// </summary>
        public MidiOutPortCaps Capabilities
        {
            get { return MidiOutPort.GetPortCapabilities(PortId); }
        }

        /// <summary>
        /// Returns the capabilities for the specified <paramref name="portId"/>.
        /// </summary>
        /// <param name="portId">An index into the list of available out ports.</param>
        /// <returns>Never returns null.</returns>
        public static MidiOutPortCaps GetPortCapabilities(int portId)
        {
            MidiOutCaps caps = new MidiOutCaps();

            int result = NativeMethods.midiOutGetDevCaps(
                new IntPtr(portId), ref caps, (uint)MemoryUtil.SizeOfMidiOutCaps);

            ThrowIfError(result);

            return new MidiOutPortCaps(ref caps);
        }

        /// <summary>
        /// Throws an exception if the <paramref name="result"/> represents an error code.
        /// </summary>
        /// <param name="result">The result of a call to one of the <see cref="NativeMethods"/> methods.</param>
        /// <exception cref="MidiOutPortException">Thrown when the <paramref name="result"/> is non-zero.</exception>
        public static void ThrowIfError(int result)
        {
            if (result != NativeMethods.MMSYSERR_NOERROR)
            {
                string msg = GetErrorText(result);

                throw new MidiOutPortException(msg);
            }
        }

        /// <summary>
        /// Lookup an error description text.
        /// </summary>
        /// <param name="result">Result of a <see cref="NativeMethods"/> call.</param>
        /// <returns>Returns the error text or an empty string. Never returns null.</returns>
        protected static string GetErrorText(int result)
        {
            if (result != NativeMethods.MMSYSERR_NOERROR)
            {
                StringBuilder builder = new StringBuilder(NativeMethods.MAXERRORLENGTH);

                int err = NativeMethods.midiOutGetErrorText(result, builder, builder.Capacity);

                if (err != NativeMethods.MMSYSERR_NOERROR)
                {
                    // TODO: log error
                }

                return builder.ToString();
            }

            return String.Empty;
        }

        #region IMidiSender Members

        /// <summary>
        /// Sends the short midi message to the Midi Out Port.
        /// </summary>
        /// <param name="data">A short midi message.</param>
        public virtual void ShortData(int data)
        {
            int result = NativeMethods.midiOutShortMsg(MidiSafeHandle, (uint)data);

            ThrowIfError(result);
        }

        /// <summary>
        /// Sends the long midi message to the Midi Out Port.
        /// </summary>
        /// <param name="buffer">The long midi message. Must not be null.</param>
        public virtual void LongData(MidiBufferStream buffer)
        {
            Contract.Requires<ArgumentNullException>(buffer != null);

            if ((buffer.HeaderFlags & NativeMethods.MHDR_PREPARED) == 0)
            {
                throw new InvalidOperationException("LongData cannot be called with a MidiBufferStream that has not been prepared.");
                //MidiBufferManager.Prepare(buffer);
            }

            int result = NativeMethods.midiOutLongMsg(MidiSafeHandle, buffer.ToIntPtr(),
                (uint)MemoryUtil.SizeOfMidiHeader);

            ThrowIfError(result);
        }

        #endregion IMidiSender Members
    }
}